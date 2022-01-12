using Markdig;
using Markdig.SyntaxHighlighting;

namespace LightWiki.ArticleEngine.MarkDown
{
    public class MdHelper : IMdHelper
    {
        private readonly MarkdownPipeline _pipeline;

        public MdHelper()
        {
            _pipeline = new MarkdownPipelineBuilder()
                .UseEmojiAndSmiley()
                .UseImageAsFigure()
                .UseAdvancedExtensions()
                .Build();
        }

        public string ConvertMdToHtml(string source)
        {
            var result = Markdown.ToHtml(source, _pipeline);
            return result;
        }
    }
}