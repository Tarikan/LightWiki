using System.Linq;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Auth;

public class AuthorizedUserProvider : IAuthorizedUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly WikiContext _wikiContext;
    private readonly AppConfiguration _appConfiguration;

    private UserContext _cachedUserContext;
    private bool _isUserContextSet;

    public AuthorizedUserProvider(
        IHttpContextAccessor httpContextAccessor,
        WikiContext wikiContext,
        AppConfiguration appConfiguration)
    {
        _httpContextAccessor = httpContextAccessor;
        _wikiContext = wikiContext;
        _appConfiguration = appConfiguration;
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
            .FirstOrDefault(c => c.Type == _appConfiguration.UserIdHeaderName)
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

        var userParty = await _wikiContext.Users.Where(u => u.Id == userId).Select(u => new int?(u.PartyId))
            .SingleOrDefaultAsync();

        if (!userParty.HasValue)
        {
            throw new UserNotFoundException();
        }

        var userContext = new UserContext
        {
            Id = userId,
            Email = userEmailClaim,
            PartyId = userParty.Value,
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