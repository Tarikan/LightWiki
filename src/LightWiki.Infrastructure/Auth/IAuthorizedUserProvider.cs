using System.Threading.Tasks;

namespace LightWiki.Infrastructure.Auth;

public interface IAuthorizedUserProvider
{
    public Task<UserContext> GetUserOrDefault();

    public Task<UserContext> GetUser();
}