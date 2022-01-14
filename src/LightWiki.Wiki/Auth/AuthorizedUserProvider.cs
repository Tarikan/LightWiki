using System.Linq;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Infrastructure.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Auth;

public class AuthorizedUserProvider : IAuthorizedUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly WikiContext _wikiContext;

    private UserContext _cachedUserContext;
    private bool _isUserContextSet;

    public AuthorizedUserProvider(IHttpContextAccessor httpContextAccessor, WikiContext wikiContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _wikiContext = wikiContext;
    }

    public async Task<UserContext> GetUserOrDefault()
    {
        if (_isUserContextSet)
        {
            return _cachedUserContext.Clone() as UserContext;
        }

        var httpContext = _httpContextAccessor.HttpContext;

        var userIdClaim = httpContext?.User
            .Claims
            .FirstOrDefault(c => c.Type == "custom:public_id")
            ?.Value;

        var canParseId = int.TryParse(userIdClaim, out var userId);

        if (!canParseId)
        {
            throw new InvalidUserIdFormatException();
        }

        var userEmailClaim = httpContext?.User
            .Claims
            .FirstOrDefault(c => c.Type == "email")?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim) ||
            string.IsNullOrWhiteSpace(userEmailClaim))
        {
            return null;
        }

        if (!await _wikiContext.Users.AnyAsync(u => u.Id == userId))
        {
            throw new UserNotFoundException();
        }

        var userContext = new UserContext
        {
            Id = userId,
            Email = userEmailClaim,
        };

        _cachedUserContext = userContext;
        _isUserContextSet = true;

        return userContext;
    }

    public async Task<UserContext> GetUser()
    {
        return await GetUserOrDefault() ?? throw new UserNotFoundException();
    }
}