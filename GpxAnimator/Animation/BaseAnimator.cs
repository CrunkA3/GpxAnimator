using GpxAnimator.Rendering;

namespace GpxAnimator.Animation;

public abstract class BaseAnimator : IAnimator
{
    public double StartTime { get; }
    public double EndTime { get; }

    protected BaseAnimator(double start, double end)
    {
        StartTime = start;
        EndTime = end;
    }

    public bool IsActive(double t) => t >= StartTime && t <= EndTime;

    public void Render(RenderContext ctx, double time)
    {
        if (!IsActive(time))
            return;

        double localT = (time - StartTime) / (EndTime - StartTime);
        RenderFrame(ctx, localT);
    }

    protected abstract void RenderFrame(RenderContext ctx, double progress);
}
