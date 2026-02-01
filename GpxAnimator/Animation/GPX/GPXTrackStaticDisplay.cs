using GpxAnimator.Rendering;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace GpxAnimator.Animation.GPX;

public class GPXTrackStaticDisplay : BaseAnimator
{
    private readonly List<List<SKPoint>> _tracks;
    private readonly SKPaint _paint;

    public GPXTrackStaticDisplay(
        GPXTracks tracks,
        int width,
        int height,
        SKColor color,
        float strokeWidth,
        double start,
        double end)
        : base(start, end)
    {
        var projector = new TrackProjector(tracks);
        _tracks = projector.ProjectAll(tracks, width, height);

        _paint = new SKPaint
        {
            Color = color,
            StrokeWidth = strokeWidth,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };
    }

    protected override void RenderFrame(RenderContext ctx, double progress)
    {
        using var path = new SKPath();

        for (int iTrack = 0; iTrack < _tracks.Count; iTrack++)
        {
            path.MoveTo(_tracks[iTrack][0]);

            for (int i = 1; i < _tracks[iTrack].Count; i++)
                path.LineTo(_tracks[iTrack][i]);
        }

        ctx.Canvas.DrawPath(path, _paint);
    }
}
