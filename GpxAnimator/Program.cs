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
double trackHoldDuration = 2.0; // Sekunden


double trackFadeEnd = trackFadeStart + trackFadeDuration + trackHoldDuration;


double trackVerticalDuration = 12.0; // Sekunden


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
    trackText: "Tag {0:N0}"
));

renderer.AddAnimator(new GPXTrackStaticDisplay(
    tracks,
    width: width,
    height: height,
    color: SKColors.Red,
    strokeWidth: 4,
    start: trackFadeStart + trackFadeDuration,
    end: trackFadeEnd
));

renderer.AddAnimator(new GPXTrackToVerticalAnimator(
    tracks,         // Deine GPX Tracks
    width,          // Breite
    height,         // Höhe
    SKColors.Red,   // Farbe
    3.0f,           // Linienbreite
    trackFadeEnd,
    trackFadeEnd + trackVerticalDuration
));



Console.WriteLine("Starte encoding");
using var encoder = new VideoEncoder(@"C:\temp\output.mp4", width, height, fps);

int totalFrames = (int)(renderer.Duration * fps);

for (int i = 0; i < totalFrames; i++)
{
    double t = i / (double)fps;

    using var surface = SKSurface.Create(new SKImageInfo(width, height));
    var canvas = surface.Canvas;

    renderer.RenderFrame(canvas, width, height, t);

    using var snapshot = surface.Snapshot();
    using var bitmap = SKBitmap.FromImage(snapshot);

    encoder.AddFrame(bitmap);
}


Console.WriteLine("Fertig");