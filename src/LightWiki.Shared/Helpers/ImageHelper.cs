using System;
using System.IO;
using SkiaSharp;

namespace LightWiki.Shared.Helpers;

public sealed class ImageHelper : IImageHelper
{
    public Stream ResizeImage(Stream imageStream, int width, int height)
    {
        using var sourceBitmap = SKBitmap.Decode(imageStream);

        using var scaledBitmap = sourceBitmap.Resize(new SKImageInfo(width, height), SKFilterQuality.Medium);
        using var scaledImage = SKImage.FromBitmap(scaledBitmap);
        var result = new MemoryStream();
        scaledImage.Encode().SaveTo(result);
        result.Position = 0;
        return result;
    }

    public Stream ResizeImage(byte[] imageBytes, int width, int height)
    {
        using var ms = new MemoryStream(imageBytes);
        using var sourceBitmap = SKBitmap.Decode(ms);

        using var scaledBitmap = sourceBitmap.Resize(new SKImageInfo(width, height), SKFilterQuality.Medium);
        using var scaledImage = SKImage.FromBitmap(scaledBitmap);
        var result = new MemoryStream();
        scaledImage.Encode().SaveTo(result);
        result.Position = 0;
        return result;
    }

    public Stream ResizeImage(Stream imageStream, int height)
    {
        using var sourceBitmap = SKBitmap.Decode(imageStream);
        var width = CalculateWidth(sourceBitmap.Width, sourceBitmap.Height, height);

        using var scaledBitmap = sourceBitmap.Resize(new SKImageInfo(width, height), SKFilterQuality.Medium);
        using var scaledImage = SKImage.FromBitmap(scaledBitmap);
        var result = new MemoryStream();
        scaledImage.Encode().SaveTo(result);
        result.Position = 0;
        return result;
    }

    public Stream ResizeImage(byte[] imageBytes, int height)
    {
        using var ms = new MemoryStream(imageBytes);
        using var sourceBitmap = SKBitmap.Decode(ms);
        var width = CalculateWidth(sourceBitmap.Width, sourceBitmap.Height, height);

        using var scaledBitmap = sourceBitmap.Resize(new SKImageInfo(width, height), SKFilterQuality.Medium);
        using var scaledImage = SKImage.FromBitmap(scaledBitmap);
        var result = new MemoryStream();
        scaledImage.Encode().SaveTo(result);
        result.Position = 0;
        return result;
    }

    private static int CalculateWidth(int currWidth, int currHeight, int height)
    {
        var proportion = Convert.ToDouble(height) / currHeight;
        return Convert.ToInt32(currWidth * proportion);
    }
}