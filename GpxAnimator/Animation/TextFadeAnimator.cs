using GpxAnimator.Animation;
using GpxAnimator.Rendering;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace GpxAnimator.Animation;


public class TextFadeAnimator : BaseAnimator
{
    private readonly string _text;
    private readonly SKFont _font;
    private readonly SKPaint _paint;

    public TextFadeAnimator(string text, double start, double end)
        : base(start, end)
    {
        _text = text;
        _font = new SKFont(SKTypeface.Default, 180);
        _paint = new SKPaint()
        {
            Color = SKColors.White,
            IsAntialias = true
        };
    }

    protected override void RenderFrame(RenderContext ctx, double progress)
    {
        byte alpha = (byte)(1 - progress * 255);
        _paint.Color = new SKColor(255, 255, 255, alpha);

        ctx.Canvas.DrawText(_text, 1080, 1080, SKTextAlign.Center, _font, _paint);
    }
}
