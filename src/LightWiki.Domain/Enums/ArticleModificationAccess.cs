namespace LightWiki.Domain.Enums
{
    public enum ArticleModificationAccess
    {
        AvailableForModification = 0,
        AvailableForAdmins = 1,
        NotAvailable = 2, // Home page
    }
}