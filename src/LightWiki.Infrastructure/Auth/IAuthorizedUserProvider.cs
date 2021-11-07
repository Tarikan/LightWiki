namespace LightWiki.Infrastructure.Auth
{
    public interface IAuthorizedUserProvider
    {
        public UserContext GetUserOrDefault();

        public UserContext GetUser();
    }
}