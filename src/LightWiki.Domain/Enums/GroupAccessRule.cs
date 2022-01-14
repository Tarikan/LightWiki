using System;

namespace LightWiki.Domain.Enums
{
    [Flags]
    public enum GroupAccessRule
    {
        None = 0,
        AddMembers = 1,
        RemoveMembers = 1 << 1,
        ModifyGroup = 1 << 2,
        RemoveGroup = 1 << 3,
        All = AddMembers | RemoveMembers | ModifyGroup | RemoveGroup,
    }
}