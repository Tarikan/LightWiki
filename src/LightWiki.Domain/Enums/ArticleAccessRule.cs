namespace LightWiki.Domain.Enums
{
    public enum ArticleAccessRule
    {
        // No access to resource
        None = 0,
        Read = 1,
        ReadWrite = 2,
        ReadWriteModify = 3,
    }
}