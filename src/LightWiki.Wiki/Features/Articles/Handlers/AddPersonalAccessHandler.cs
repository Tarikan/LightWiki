using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Domain.Models;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace LightWiki.Features.Articles.Handlers;

public class AddPersonalAccessHandler : IRequestHandler<AddPersonalAccess, OneOf<Success, Fail>>
{
    private readonly WikiContext _wikiContext;

    public AddPersonalAccessHandler(WikiContext wikiContext)
    {
        _wikiContext = wikiContext;
    }

    public async Task<OneOf<Success, Fail>> Handle(AddPersonalAccess request, CancellationToken cancellationToken)
    {
        var user = await _wikiContext.Users.FindAsync(request.UserId);

        var access = new ArticlePersonalAccessRule
        {
            User = user,
            ArticleId = request.ArticleId,
            ArticleAccessRule = request.AccessRule,
        };

        await _wikiContext.ArticlePersonalAccessRules.AddAsync(access, cancellationToken);

        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}