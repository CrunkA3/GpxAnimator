// See https://aka.ms/new-console-template for more information
using GpxAnimator.Animation;
using GpxAnimator.Animation.GPX;
using GpxAnimator.Encoding;
using GpxAnimator.Rendering;
using SkiaSharp;

Console.WriteLine("Lade GPX-Dateien");
// GPX-Dateien aus ordner gpx_files laden
var tracks = GPXTracks.LoadTracks();



int width = 2160;
int height = 2160;
int fps = 60;

double trackFadeStart = 2.0; // Sekunden
double trackFadeDuration = 12.0; // Sekunden
double trackHoldDuration = 1.0; // Sekunden


double trackFadeEnd = trackFadeStart + trackFadeDuration + trackHoldDuration;


double trackToVerticalDuration = 6.0; // Sekunden

double verticalStart = trackFadeEnd + trackToVerticalDuration;
double verticalDuration = 6.0; // Sekunden
double verticalHoldDuration = 5.0; // Sekunden

var renderer = new FrameRenderer();


Console.WriteLine("Add Animators");

renderer.AddAnimator(new TextFadeAnimator("run the date", 0, 3));
renderer.AddAnimator(new GPXTrackPathAnimator(
    tracks,
    width: width,
    height: height,
    color: SKColors.Red,
    strokeWidth: 4,
    start: trackFadeStart,
    end: trackFadeStart + trackFadeDuration,
    hold: trackHoldDuration,
    trackText: "Tag {0:N0}"
));



// GPX Track to Vertikal Animation
renderer.AddAnimator(new GPXTrackToVerticalAnimator(
    tracks,         // Deine GPX Tracks
    width,          // Breite
    height,         // Höhe
    SKColors.Red,   // Farbe
    3.0f,           // Linienbreite am Anfang
    8.0f,           // Linienbreite am Ende
    trackFadeEnd,
    verticalStart
));


// GPX Vertikal Animation
renderer.AddAnimator(new GPXVerticalAnimator(
    tracks,         // Deine GPX Tracks
    width,          // Breite
    height,         // Höhe
    SKColors.Red,   // Farbe
    8.0f,           // Linienbreite
    16.0f,          // Kreisbreite
    verticalStart,
    verticalStart + verticalDuration,
    verticalHoldDuration,
    "{0:N0} km"
));



Console.WriteLine("Starte encoding");
using var encoder = new VideoEncoder(@"C:\temp\output.mp4", width, height, fps);

int totalFrames = (int)(renderer.Duration * fps);
var startTime = DateTime.Now;

for (int i = 0; i < totalFrames; i++)
{
    double t = i / (double)fps;

    using var surface = SKSurface.Create(new SKImageInfo(width, height));
    var canvas = surface.Canvas;

    renderer.RenderFrame(canvas, width, height, t);

    using var snapshot = surface.Snapshot();
    using var bitmap = SKBitmap.FromImage(snapshot);

    encoder.AddFrame(bitmap);

    // Fortschrittsanzeige
    if (i % 10 == 0 || i == totalFrames - 1)
    {
        double progress = (i + 1) / (double)totalFrames * 100;
        var elapsed = DateTime.Now - startTime;
        var estimatedTotal = elapsed.TotalSeconds / (i + 1) * totalFrames;
        var remaining = TimeSpan.FromSeconds(estimatedTotal - elapsed.TotalSeconds);

        Console.Write($"\rFrame {i + 1}/{totalFrames} ({progress:F1}%) - Verbleibend: {remaining:mm\\:ss}");
    }
}

Console.WriteLine();
Console.WriteLine("Fertig");