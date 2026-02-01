using System;
using System.Collections.Generic;
using System.Text;
using FFMediaToolkit;
using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;
using SkiaSharp;


namespace GpxAnimator.Encoding;

public class VideoEncoder : IDisposable
{
    private readonly MediaOutput _output;
    private readonly int _width;
    private readonly int _height;

    public VideoEncoder(string path, int width, int height, int fps)
    {
        _width = width;
        _height = height;

        var settings = new VideoEncoderSettings(width, height, fps, VideoCodec.H264)
        {
            EncoderPreset = EncoderPreset.Medium,
            CRF = 18
        };

        _output = MediaBuilder.CreateContainer(path).WithVideo(settings).Create();
    }

    public void AddFrame(SKBitmap bitmap)
    {
        var data = bitmap.Bytes; var imageData = ImageData.FromArray(data, ImagePixelFormat.Bgra32, _width, _height); _output.Video.AddFrame(imageData);
    }

    public void Dispose()
    {
        _output.Dispose();
        GC.SuppressFinalize(this);
    }
}
