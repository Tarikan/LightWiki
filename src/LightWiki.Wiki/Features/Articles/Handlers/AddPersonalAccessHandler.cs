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
        var access = await _wikiContext.ArticlePersonalAccessRules.FirstOrDefaultAsync(
            r =>
            r.UserId == request.UserId &&
            r.ArticleId == request.ArticleId,
            cancellationToken);

        access ??= new ArticlePersonalAccessRule();

        access.UserId = request.UserId;
        access.ArticleId = request.ArticleId;
        access.ArticleAccessRule = request.AccessRule;

        _wikiContext.ArticlePersonalAccessRules.Update(access);

        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}