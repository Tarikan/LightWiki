using System.Linq;
using System.Security.Cryptography;

namespace LightWiki.Shared.Helpers;

public class HashHelper : IHashHelper
{
    public string GetMd5Hash(byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0)
        {
            return null;
        }

        using var md5 = MD5.Create();
        return string.Join(string.Empty, md5.ComputeHash(bytes).Select(x => x.ToString("X2")));
    }
}