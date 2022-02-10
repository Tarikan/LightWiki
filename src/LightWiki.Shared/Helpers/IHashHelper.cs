namespace LightWiki.Shared.Helpers;

public interface IHashHelper
{
    string GetMd5Hash(byte[] bytes);
}