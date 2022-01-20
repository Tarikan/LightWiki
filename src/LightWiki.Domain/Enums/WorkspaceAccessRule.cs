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
        ModifyArticle = 1 << 2,
        ManageWorkspace = 1 << 3,
        All = Browse | CreateArticle | ModifyArticle | ManageWorkspace,
    }
}