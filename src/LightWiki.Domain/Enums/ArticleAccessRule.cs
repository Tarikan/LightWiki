using System;

namespace LightWiki.Domain.Enums
{
    [Flags]
    public enum ArticleAccessRule : int
    {
        None = 0,
        Read = 1,
        Write = 1 << 1,
        Modify = 1 << 2,
        All = Read | Write | Modify,
    }
}