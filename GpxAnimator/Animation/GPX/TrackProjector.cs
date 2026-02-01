using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace GpxAnimator.Animation.GPX;

public class TrackProjector
{
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

        return track.Points.Select(p =>
        {
            float x = (float)((p.Lon - _minLon) / lonRange * availableWidth) + Margin;
            float y = (float)((1 - (p.Lat - _minLat) / latRange) * availableHeight) + Margin;
            return new SKPoint(x, y);
        }).ToList();
    }

    public List<List<SKPoint>> ProjectAll(GPXTracks tracks, int width, int height)
    {
        return tracks.Select(track => Project(track, width, height)).ToList();
    }
}


