using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace LightWiki.Features.Articles.Handlers;

public class RemoveArticleAccessHandler : IRequestHandler<RemoveArticleAccess, OneOf<Success, Fail>>
{
    private readonly WikiContext _wikiContext;

    public RemoveArticleAccessHandler(WikiContext wikiContext)
    {
        _wikiContext = wikiContext;
    }

    public async Task<OneOf<Success, Fail>> Handle(RemoveArticleAccess request, CancellationToken cancellationToken)
    {
        var access = await _wikiContext.ArticleAccesses
            .Include(a => a.Party)
            .ThenInclude(a => a.Groups)
            .SingleAsync(
                r => r.PartyId == request.PartyId &&
                     r.ArticleId == request.ArticleId,
                cancellationToken);

        if (access.Party.Groups.Any() && access.Party.Groups.First().GroupType != GroupType.Regular)
        {
            return new Fail("Cannot remove access from default group", FailCode.BadRequest);
        }

        _wikiContext.ArticleAccesses.Remove(access);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}