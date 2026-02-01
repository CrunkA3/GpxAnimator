using GpxAnimator.Rendering;

namespace GpxAnimator.Animation;

public interface IAnimator
{
    double StartTime { get; }
    double EndTime { get; }

    /// <summary>
    /// Wird pro Frame aufgerufen.
    /// </summary>
    void Render(RenderContext ctx, double time);
}
