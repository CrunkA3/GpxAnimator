using GpxAnimator.Rendering;
using SkiaSharp;

namespace GpxAnimator.Animation.GPX;

public class GPXVerticalAnimator : BaseAnimator
{
    private readonly List<List<float>> _trackDistances;
    private readonly SKPaint _paint;
    private readonly int _height;
    private readonly int _width;
    private readonly float _circleWidth;

    public float Margin { get; set; } = 100f;

    public GPXVerticalAnimator(
        GPXTracks tracks,
        int width,
        int height,
        SKColor color,
        float strokeWidth,
        float circleWidth,
        double start,
        double end,
        double hold)
        : base(start, end, hold)
    {
        var distances = TrackProjector.ToDistances(tracks);
        _trackDistances = distances;

        _height = height;
        _width = width;
        _circleWidth = circleWidth;

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

        var availableHeight = _height - 2 * Margin;
        var availableWidth = _width - 2 * Margin;

        float bottomY = availableHeight + Margin;

        var maxDistance = _trackDistances.Max(distances => distances.Last());
        var distanceScale = availableHeight / maxDistance;

        for (int iTrack = 0; iTrack < _trackDistances.Count; iTrack++)
        {
            var track = _trackDistances[iTrack];
            if (track.Count < 2) continue;

            // Berechne die durchschnittliche X-Position für die vertikale Linie
            float targetX = Margin + availableWidth / _trackDistances.Count * (iTrack + 0.5f);

            // Berechne die tatsächliche Länge des ursprünglichen Tracks
            var distances = _trackDistances[iTrack]; // Kumulative Distanzen
            float totalDistance = distances.Last();

            // Berechne die Y-Position für den Start der vertikalen Linie (zentriert)
            float topY = bottomY - totalDistance * distanceScale;

            // Zeichne die vertikale Linie
            path.MoveTo(targetX, bottomY);
            path.LineTo(targetX, topY);

            // Berechne die aktuelle Höhe basierend auf dem Fortschritt
            ctx.Canvas.DrawCircle(targetX, bottomY - Math.Min((float)(maxDistance * progress), totalDistance) * distanceScale, _circleWidth, _paint);
        }

        ctx.Canvas.DrawPath(path, _paint);
    }
}
