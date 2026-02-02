using GpxAnimator.Rendering;
using SkiaSharp;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace GpxAnimator.Animation.GPX;


public class GPXTrackPathAnimator : BaseAnimator
{
    private readonly List<List<SKPoint>> _tracks;
    private readonly SKPaint _paint;
    private readonly SKPaint _textPaint;
    private readonly SKFont _font;

    private readonly string _trackText;
    private readonly int _totalPoints;
    private readonly double[] _trackProgressThresholds;

    public GPXTrackPathAnimator(
        GPXTracks tracks,
        int width,
        int height,
        SKColor color,
        float strokeWidth,
        double start,
        double end,
        double hold,
        string trackText)
        : base(start, end, hold)
    {
        var projector = new TrackProjector(tracks);
        _tracks = projector.ProjectAll(tracks, width, height);

        // Calculate total points across all tracks
        _totalPoints = _tracks.Sum(t => t.Count);

        // Calculate cumulative progress thresholds for each track based on point count
        _trackProgressThresholds = new double[_tracks.Count + 1];
        _trackProgressThresholds[0] = 0;
        for (int i = 0; i < _tracks.Count; i++)
        {
            _trackProgressThresholds[i + 1] = _trackProgressThresholds[i] + (double)_tracks[i].Count / _totalPoints;
        }

        _font = new SKFont(SKTypeface.Default, 128);

        _paint = new SKPaint
        {
            Color = color,
            StrokeWidth = strokeWidth,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };

        _textPaint = new SKPaint
        {
            Color = color,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        _trackText = trackText;
    }

    protected override void RenderFrame(RenderContext ctx, double progress)
    {
        if (progress == 0) return;

        // Find which track we're currently animating based on progress
        int currentTrackIndex = 0;
        for (int i = 0; i < _tracks.Count; i++)
        {
            if (progress <= _trackProgressThresholds[i + 1])
            {
                currentTrackIndex = i;
                break;
            }
        }

        // Calculate progress within the current track
        double trackProgress = 0;
        double thresholdRange = _trackProgressThresholds[currentTrackIndex + 1] - _trackProgressThresholds[currentTrackIndex];
        if (thresholdRange > 0)
        {
            trackProgress = Math.Clamp((progress - _trackProgressThresholds[currentTrackIndex]) / thresholdRange, 0, 1);
        }

        using var path = new SKPath();

        // Render all completed tracks
        for (int iTrack = 0; iTrack < currentTrackIndex; iTrack++)
        {
            if (_tracks[iTrack].Count < 2) continue;

            path.MoveTo(_tracks[iTrack][0]);
            for (int iPoint = 1; iPoint < _tracks[iTrack].Count; iPoint++)
                path.LineTo(_tracks[iTrack][iPoint]);
        }

        // Render current track with progress
        if (currentTrackIndex < _tracks.Count && _tracks[currentTrackIndex].Count >= 2)
        {
            var visiblePointsCount = Math.Max(1, (int)(_tracks[currentTrackIndex].Count * trackProgress));

            path.MoveTo(_tracks[currentTrackIndex][0]);
            for (int iPoint = 1; iPoint < visiblePointsCount; iPoint++)
                path.LineTo(_tracks[currentTrackIndex][iPoint]);

            // Show current position with a circle
            if (visiblePointsCount > 0)
            {
                ctx.Canvas.DrawCircle(_tracks[currentTrackIndex][visiblePointsCount - 1], _paint.StrokeWidth * 2f, new SKPaint
                {
                    Color = _paint.Color,
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill
                });
            }
        }

        ctx.Canvas.DrawPath(path, _paint);

        RenderTrackText(ctx, currentTrackIndex + 1);
    }

    private void RenderTrackText(RenderContext ctx, int trackNumber)
    {
        if (string.IsNullOrEmpty(_trackText)) return;

        ctx.Canvas.DrawText(string.Format(_trackText, trackNumber), 50, 100, _font, _textPaint);
    }
}
