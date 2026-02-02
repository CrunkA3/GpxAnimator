using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace GpxAnimator.Animation.GPX;

public class TrackProjector
{
    private const double EarthRadiusMeters = 6_371_000;

    private readonly double _minLat;
    private readonly double _maxLat;
    private readonly double _minLon;
    private readonly double _maxLon;

    public float Margin { get; set; } = 100f;

    public TrackProjector(GPXTrack track)
    {
        _minLat = track.MinLat;
        _maxLat = track.MaxLat;
        _minLon = track.MinLon;
        _maxLon = track.MaxLon;
    }

    public TrackProjector(GPXTracks tracks)
    {
        _minLat = tracks.MinLat;
        _maxLat = tracks.MaxLat;
        _minLon = tracks.MinLon;
        _maxLon = tracks.MaxLon;
    }

    public List<SKPoint> Project(GPXTrack track, int width, int height)
    {
        double latRange = _maxLat - _minLat;
        double lonRange = _maxLon - _minLon;

        float availableWidth = width - 2 * Margin;
        float availableHeight = height - 2 * Margin;

        float latScale = (float)(availableHeight / latRange);
        float lonScale = (float)(availableWidth / lonRange);

        float scale = Math.Min(latScale, lonScale);

        return track.Points.Select(p =>
        {
            float x = (float)((p.Lon - _minLon) * scale) + Margin;
            float y = (float)((latRange - (p.Lat - _minLat)) * scale) + Margin;

            return new SKPoint(x, y);
        }).ToList();
    }

    public List<List<SKPoint>> ProjectAll(GPXTracks tracks, int width, int height)
    {
        return tracks.Select(track => Project(track, width, height)).ToList();
    }


    public static List<float> ToDistances(GPXTrack track)
    {
        var distances = new List<float>(track.Points.Count) { 0.0f };
        double distance = 0;

        for (int i = 1; i < track.Points.Count; i++)
        {
            double phi1 = Math.PI / 180.0 * track.Points[i - 1].Lat;
            double phi2 = Math.PI / 180.0 * track.Points[i].Lat;
            double deltaPhi = Math.PI / 180.0 * (track.Points[i].Lat - track.Points[i - 1].Lat);
            double deltaLambda = Math.PI / 180.0 * (track.Points[i].Lon - track.Points[i - 1].Lon);

            double a =
                Math.Sin(deltaPhi / 2) * Math.Sin(deltaPhi / 2) +
                Math.Cos(phi1) * Math.Cos(phi2) *
                Math.Sin(deltaLambda / 2) * Math.Sin(deltaLambda / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            distance += EarthRadiusMeters * c;

            // Store the distance in place of lat for this example
            distances.Add((float)distance);
        }

        return distances;

    }


    public static List<List<float>> ToDistances(GPXTracks tracks)
    {
        return tracks.Select(track => ToDistances(track)).ToList();
    }
}
