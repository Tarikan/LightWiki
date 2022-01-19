namespace LightWiki.Domain.Enums
{
    public static class RuleConverter
    {
        public static ArticleAccessRule Convert(WorkspaceAccessRule workspaceAccessRule)
        {
            var result = ArticleAccessRule.None;

            if (workspaceAccessRule.HasFlag(WorkspaceAccessRule.ReadArticle))
            {
                result = result | ArticleAccessRule.Read;
            }

            if (workspaceAccessRule.HasFlag(WorkspaceAccessRule.ModifyArticle))
            {
                result |= ArticleAccessRule.Modify | ArticleAccessRule.Write;
            }

            return result;
        }
    }
}