using GpxAnimator.Rendering;
using SkiaSharp;

namespace GpxAnimator.Animation.GPX;

public class GPXTrackToVerticalAnimator : BaseAnimator
{
    private readonly List<List<SKPoint>> _tracks;
    private readonly List<List<float>> _trackDistances;
    private readonly SKPaint _paint;
    private readonly int _height;
    private readonly int _width;
    private readonly float _strokeWidthStart;
    private readonly float _strokeWidthEnd;

    public float Margin { get; set; } = 100f;

    public GPXTrackToVerticalAnimator(
        GPXTracks tracks,
        int width,
        int height,
        SKColor color,
        float strokeWidthStart,
        float strokeWidthEnd,
        double start,
        double end)
        : base(start, end)
    {
        var projector = new TrackProjector(tracks);
        _tracks = projector.ProjectAll(tracks, width, height);

        var distances = TrackProjector.ToDistances(tracks);
        _trackDistances = distances;

        _height = height;
        _width = width;
        _strokeWidthStart = strokeWidthStart;
        _strokeWidthEnd = strokeWidthEnd;

        _paint = new SKPaint
        {
            Color = color,
            StrokeWidth = strokeWidthStart,
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

        for (int iTrack = 0; iTrack < _tracks.Count; iTrack++)
        {
            var track = _tracks[iTrack];
            if (track.Count < 2) continue;

            // Berechne die durchschnittliche X-Position für die vertikale Linie
            float targetX = Margin + availableWidth / _tracks.Count * (iTrack + 0.5f);

            // Berechne die tatsächliche Länge des ursprünglichen Tracks
            var distances = _trackDistances[iTrack]; // Kumulative Distanzen
            float totalDistance = distances.Last();

            // Berechne die Y-Position für den Start der vertikalen Linie (zentriert)
            float topY = bottomY - totalDistance * distanceScale;

            // Interpoliere jeden Punkt zwischen seiner ursprünglichen Position und der vertikalen Linie
            var interpolatedPoints = new List<SKPoint>();

            for (int i = 0; i < track.Count; i++)
            {
                var originalPoint = track[i];

                // Berechne die Position auf der vertikalen Linie basierend auf der kumulativen Distanz
                float targetY = topY + distances[i] * distanceScale;

                // Interpoliere zwischen ursprünglicher Position und Zielposition
                float interpolatedX = originalPoint.X + (targetX - originalPoint.X) * (float)progress;
                float interpolatedY = originalPoint.Y + (targetY - originalPoint.Y) * (float)progress;

                interpolatedPoints.Add(new SKPoint(interpolatedX, interpolatedY));
            }

            // Zeichne den interpolierten Track
            if (interpolatedPoints.Count > 0)
            {
                path.MoveTo(interpolatedPoints[0]);
                for (int i = 1; i < interpolatedPoints.Count; i++)
                {
                    path.LineTo(interpolatedPoints[i]);
                }
            }
        }

        _paint.StrokeWidth = _strokeWidthStart + (_strokeWidthEnd - _strokeWidthStart) * (float)progress;

        ctx.Canvas.DrawPath(path, _paint);
    }
}
