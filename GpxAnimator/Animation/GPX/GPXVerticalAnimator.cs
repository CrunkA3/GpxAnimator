using GpxAnimator.Rendering;
using SkiaSharp;

namespace GpxAnimator.Animation.GPX;

public class GPXVerticalAnimator : BaseAnimator
{
    private readonly List<List<float>> _trackDistances;
    private readonly SKPaint _paint;
    private readonly SKPaint _circlePaint;
    private readonly SKPaint _textPaint;
    private readonly SKFont _font;

    private readonly int _height;
    private readonly int _width;
    private readonly float _circleWidth;
    private readonly string _distanceText;

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
        double hold,
        string distanceText)
        : base(start, end, hold)
    {
        var distances = TrackProjector.ToDistances(tracks);
        _trackDistances = distances;

        _height = height;
        _width = width;
        _circleWidth = circleWidth;
        _distanceText = distanceText;

        _font = new SKFont(SKTypeface.Default, 128);

        _paint = new SKPaint
        {
            Color = color,
            StrokeWidth = strokeWidth,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };

        _circlePaint = new SKPaint
        {
            Color = color,
            StrokeWidth = strokeWidth,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        _textPaint = new SKPaint
        {
            Color = color,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
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

        // Offset-Faktor für die Versetzung (z.B. 0.2 bedeutet 20% Versetzung)
        double staggerAmount = 0.15;

        // Aktuelle Gesamtdistanz, die bereits gezeichnet wurde
        double currentDistance = 0;

        for (int iTrack = 0; iTrack < _trackDistances.Count; iTrack++)
        {
            var track = _trackDistances[iTrack];
            if (track.Count < 2) continue;

            // Berechne den individuellen Progress für diesen Track
            double trackOffset = (iTrack / (double)_trackDistances.Count) * staggerAmount;
            double trackProgress = (progress - trackOffset) / (1.0 - staggerAmount);
            trackProgress = Math.Clamp(trackProgress, 0.0, 1.0);

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

            // Berechne die aktuelle Höhe basierend auf dem individuellen Fortschritt
            if (trackProgress > 0)
            {
                ctx.Canvas.DrawCircle(targetX, bottomY - Math.Min((float)(maxDistance * trackProgress), totalDistance) * distanceScale, _circleWidth, _circlePaint);
            }

            currentDistance += totalDistance * trackProgress;
        }

        ctx.Canvas.DrawPath(path, _paint);
        RenderDistanceText(ctx, currentDistance / 1_000);
    }
    private void RenderDistanceText(RenderContext ctx, double distance)
    {
        if (string.IsNullOrEmpty(_distanceText)) return;

        ctx.Canvas.DrawText(string.Format(_distanceText, distance), 50, 100, _font, _textPaint);
    }
}
