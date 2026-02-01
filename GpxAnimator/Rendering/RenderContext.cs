using SkiaSharp;

namespace GpxAnimator.Rendering;

public class RenderContext
{
    public SKCanvas Canvas { get; }
    public int Width { get; }
    public int Height { get; }

    public RenderContext(SKCanvas canvas, int width, int height)
    {
        Canvas = canvas;
        Width = width;
        Height = height;
    }
}
