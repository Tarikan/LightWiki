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

public class AddArticleAccessHandler : IRequestHandler<AddArticleAccess, OneOf<Success, Fail>>
{
    private readonly WikiContext _wikiContext;

    public AddArticleAccessHandler(WikiContext wikiContext)
    {
        _wikiContext = wikiContext;
    }

    public async Task<OneOf<Success, Fail>> Handle(AddArticleAccess request, CancellationToken cancellationToken)
    {
        var access = await _wikiContext.ArticleAccesses
            .SingleOrDefaultAsync(
                r => r.PartyId == request.PartyId &&
                     r.ArticleId == request.ArticleId,
                cancellationToken);

        access ??= new ArticleAccess
        {
            PartyId = request.PartyId,
            ArticleId = request.ArticleId,
        };

        access.ArticleAccessRule = request.ArticleAccessRule;

        _wikiContext.ArticleAccesses.Update(access);

        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}