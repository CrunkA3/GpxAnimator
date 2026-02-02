using GpxAnimator.Rendering;

namespace GpxAnimator.Animation;

public abstract class BaseAnimator : IAnimator
{
    public double StartTime { get; }
    public double EndTime { get; }

    public double? HoldTime { get; }

    protected BaseAnimator(double start, double end)
    {
        StartTime = start;
        EndTime = end;
        HoldTime = 0;
    }
    protected BaseAnimator(double start, double end, double? holdTime)
    {
        StartTime = start;
        EndTime = end;
        HoldTime = holdTime;
    }

    public bool IsActive(double t) => t >= StartTime && t <= (EndTime + HoldTime.GetValueOrDefault(t));

    public void Render(RenderContext ctx, double time)
    {
        if (!IsActive(time))
            return;

        double localT = Math.Min((time - StartTime) / (EndTime - StartTime), 1);
        RenderFrame(ctx, localT);
    }

    protected abstract void RenderFrame(RenderContext ctx, double progress);
}
