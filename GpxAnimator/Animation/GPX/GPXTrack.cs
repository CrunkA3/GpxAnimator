using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace GpxAnimator.Animation.GPX;

public class GPXTrack
{
    public List<(double Lat, double Lon)> Points { get; } = [];

    public double MinLat { get; private set; }
    public double MaxLat { get; private set; }
    public double MinLon { get; private set; }
    public double MaxLon { get; private set; }

    public GPXTrack(string path)
    {
        var doc = XDocument.Load(path);        

        var ns = XNamespace.Get("http://www.topografix.com/GPX/1/1");

        var pts = doc.Descendants(ns + "trkpt")
            .Select(x => (
                Lat: double.Parse(x.Attribute("lat")!.Value, System.Globalization.CultureInfo.InvariantCulture),
                Lon: double.Parse(x.Attribute("lon")!.Value, System.Globalization.CultureInfo.InvariantCulture)
            ));

        Points = [.. pts];

        MinLat = Points.Min(p => p.Lat);
        MaxLat = Points.Max(p => p.Lat);
        MinLon = Points.Min(p => p.Lon);
        MaxLon = Points.Max(p => p.Lon);
    }
}
