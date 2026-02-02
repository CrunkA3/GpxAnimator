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

    private readonly double _fontSizeStart;
    private readonly double _fontSizeEnd;

    public TextFadeAnimator(string text, double start, double end, double fontSizeStart, double fontSizeEnd )
        : base(start, end)
    {
        _text = text;
        _font = new SKFont(SKTypeface.Default, (float)fontSizeStart);
        _paint = new SKPaint()
        {
            Color = SKColors.White,
            IsAntialias = true
        };
        _fontSizeStart = fontSizeStart;
        _fontSizeEnd = fontSizeEnd;
    }

    protected override void RenderFrame(RenderContext ctx, double progress)
    {
        byte alpha = (byte)(1 - progress * 255);
        _paint.Color = new SKColor(255, 255, 255, alpha);
        _font.Size = (float)(_fontSizeStart + (_fontSizeEnd - _fontSizeStart) * progress);
        ctx.Canvas.DrawText(_text, 1080, 1080, SKTextAlign.Center, _font, _paint);
    }
}
