using System.Collections.Generic;

namespace LightWiki.ArticleEngine.Patches
{
    public interface IPatchHelper
    {
        string CreatePatch(string source, string target);

        string ApplyPatch(string patch, string text);

        string ApplyPatches(List<string> patches, string text);
    }
}