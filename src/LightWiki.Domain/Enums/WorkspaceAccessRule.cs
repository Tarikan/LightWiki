using System;

namespace LightWiki.Domain.Enums
{
    [Flags]
    public enum WorkspaceAccessRule
    {
        None = 0,
        Browse = 1,
        CreateArticle = 1 << 1,
        // will override article access rules
        ReadArticle = 1 << 2,
        ModifyArticle = 1 << 3,
        ManageWorkspace = 1 << 4,
        All = Browse | CreateArticle | ReadArticle | ModifyArticle | ManageWorkspace,
    }
}