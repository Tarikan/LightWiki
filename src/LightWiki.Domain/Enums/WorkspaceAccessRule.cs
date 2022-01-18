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
        AddMember = 1 << 3,
        RemoveMember = 1 << 4,
        ManageWorkspace = 1 << 5,
        All = Browse | CreateArticle | ModifyArticle | AddMember | RemoveMember | ManageWorkspace,
    }
}