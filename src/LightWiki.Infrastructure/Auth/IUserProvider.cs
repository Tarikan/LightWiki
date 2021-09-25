namespace LightWiki.Infrastructure.Auth
{
    public interface IUserProvider
    {
        public UserContext GetUserOrDefault();

        public UserContext GetUser();
    }
}