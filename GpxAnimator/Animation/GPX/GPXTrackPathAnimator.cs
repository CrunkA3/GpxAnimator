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

        // progress = 0..1
        int visibleTracksCount = Math.Min((int)(_tracks.Count * progress) + 1, _tracks.Count);
        double subProgress = Math.Clamp((progress - (visibleTracksCount - 1) * (1f / _tracks.Count)) / (1f / _tracks.Count), 0, 1);



        using var path = new SKPath();

        for (int iTrack = 0; iTrack < visibleTracksCount; iTrack++)
        {
            var visiblePointsCount = iTrack < visibleTracksCount - 1 ? _tracks[iTrack].Count : (int)(_tracks[iTrack].Count * subProgress);
            if (visiblePointsCount < 2) continue;

            path.MoveTo(_tracks[iTrack][0]);

            for (int iPoint = 1; iPoint < visiblePointsCount; iPoint++)
                path.LineTo(_tracks[iTrack][iPoint]);


            // show current position with a circle
            if (iTrack == visibleTracksCount - 1)
            {
                ctx.Canvas.DrawCircle(_tracks[iTrack][visiblePointsCount - 1], _paint.StrokeWidth * 2f, new SKPaint
                {
                    Color = _paint.Color,
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill
                });
            }

        }
        ctx.Canvas.DrawPath(path, _paint);

        RenderTrackText(ctx, visibleTracksCount);
    }

    private void RenderTrackText(RenderContext ctx, int trackNumber)
    {
        if (string.IsNullOrEmpty(_trackText)) return;

        ctx.Canvas.DrawText(string.Format(_trackText, trackNumber), 50, 100, _font, _textPaint);
    }
}
