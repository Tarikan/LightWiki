using System.Collections.Generic;
using DiffMatchPatch;

namespace LightWiki.ArticleEngine.Patches
{
    public class ArticleCreator
    {
        public ArticleCreator()
        {
            _diffMatchPatch = new diff_match_patch();
        }

        private readonly diff_match_patch _diffMatchPatch;

        public string CreatePatch(string source, string target)
        {
            var diffs = _diffMatchPatch.diff_main(source, target);

            var patches = _diffMatchPatch.patch_make(source, diffs);

            var result = _diffMatchPatch.patch_toText(patches);

            return result;
        }

        public string ApplyPatch(string patch, string text)
        {
            var patches = _diffMatchPatch.patch_fromText(patch);

            var result = _diffMatchPatch.patch_apply(patches, text);

            return result[0] as string;
        }

        public string ApplyPatches(List<string> patches, string text)
        {
            var result = text;

            foreach (var patch in patches)
            {
                var patchList = _diffMatchPatch.patch_fromText(patch);

                result = _diffMatchPatch.patch_apply(patchList, result)[0] as string;
            }

            return result;
        }
    }
}