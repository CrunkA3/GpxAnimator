using GpxAnimator.Animation;
using SkiaSharp;


namespace GpxAnimator.Rendering;

public class FrameRenderer
{
    private readonly List<IAnimator> _animators = [];

    public double Duration => _animators.Max(x => x.EndTime + x.HoldTime.GetValueOrDefault(0));

    public void AddAnimator(IAnimator animator)
        => _animators.Add(animator);

    public void RenderFrame(SKCanvas canvas, int width, int height, double time)
    {
        var ctx = new RenderContext(canvas, width, height);

        canvas.Clear(SKColors.Transparent);

        foreach (var animator in _animators)
            animator.Render(ctx, time);
    }
}
