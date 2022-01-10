using System.Linq;
using LightWiki.Infrastructure.Auth;
using Microsoft.AspNetCore.Http;

namespace LightWiki.Wiki.Api.Auth;

public class AuthorizedUserProvider : IAuthorizedUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizedUserProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public UserContext GetUserOrDefault()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        var userIdClaim = httpContext?.User
            .Claims
            .FirstOrDefault(c => c.Type == "custom:user_id")
            ?.Value;

        var userEmailClaim = httpContext?.User
            .Claims
            .FirstOrDefault(c => c.Type == "email")?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim) ||
            string.IsNullOrWhiteSpace(userEmailClaim))
        {
            return null;
        }

        return new UserContext
        {
            Id = int.Parse(userIdClaim),
            Email = userEmailClaim,
        };
    }

    public UserContext GetUser()
    {
        return GetUserOrDefault() ?? throw new UserNotFoundException();
    }
}