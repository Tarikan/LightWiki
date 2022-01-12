using System.Linq;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Infrastructure.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Wiki.Api.Auth;

public class AuthorizedUserProvider : IAuthorizedUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly WikiContext _wikiContext;

    public AuthorizedUserProvider(IHttpContextAccessor httpContextAccessor, WikiContext wikiContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _wikiContext = wikiContext;
    }

    public async Task<UserContext> GetUserOrDefault()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        var userIdClaim = httpContext?.User
            .Claims
            .FirstOrDefault(c => c.Type == "custom:public_id")
            ?.Value;

        var userEmailClaim = httpContext?.User
            .Claims
            .FirstOrDefault(c => c.Type == "email")?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim) ||
            string.IsNullOrWhiteSpace(userEmailClaim))
        {
            return null;
        }

        var user = await _wikiContext.Users.SingleOrDefaultAsync(u => u.PublicId == userIdClaim);

        if (user is null)
        {
            return null;
        }

        return new UserContext
        {
            Id = user.Id,
            Email = userEmailClaim,
        };
    }

    public async Task<UserContext> GetUser()
    {
        return await GetUserOrDefault() ?? throw new UserNotFoundException();
    }
}