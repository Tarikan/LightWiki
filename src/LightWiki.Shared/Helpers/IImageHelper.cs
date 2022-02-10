using System.IO;

namespace LightWiki.Shared.Helpers;

public interface IImageHelper
{
    public Stream ResizeImage(Stream imageStream, int width, int height);

    public Stream ResizeImage(byte[] imageBytes, int width, int height);

    Stream ResizeImage(Stream imageStream, int height);

    Stream ResizeImage(byte[] imageBytes, int height);
}