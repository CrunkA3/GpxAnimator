namespace GpxAnimator.Animation.GPX;

public class GPXTracks : List<GPXTrack>
{
    const string directory = "gpx_files";


    public double MinLat { get; private set; }
    public double MaxLat { get; private set; }
    public double MinLon { get; private set; }
    public double MaxLon { get; private set; }


    public static GPXTracks LoadTracks()
    {
        var tracks = new GPXTracks();
        var files = Directory
            .GetFiles(directory, "*.gpx")
            .Order()
            .ToArray();

        foreach (var file in files)
        {
            var track = new GPXTrack(file);
            tracks.Add(track);
        }

        if (tracks.Count > 0)
        {
            tracks.MinLat = tracks.Min(t => t.MinLat);
            tracks.MaxLat = tracks.Max(t => t.MaxLat);
            tracks.MinLon = tracks.Min(t => t.MinLon);
            tracks.MaxLon = tracks.Max(t => t.MaxLon);
        }

        return tracks;
    }
}
