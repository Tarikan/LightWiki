using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace LightWiki.Features.Articles.Handlers;

public class RemoveGroupAccessHandler : IRequestHandler<RemoveGroupAccess, OneOf<Success, Fail>>
{
    private readonly WikiContext _wikiContext;

    public RemoveGroupAccessHandler(WikiContext wikiContext)
    {
        _wikiContext = wikiContext;
    }

    public async Task<OneOf<Success, Fail>> Handle(RemoveGroupAccess request, CancellationToken cancellationToken)
    {
        var access = await _wikiContext.ArticleGroupAccessRules.SingleAsync(
            r => r.GroupId == request.GroupId &&
                 r.ArticleId == request.ArticleId,
            cancellationToken);

        _wikiContext.ArticleGroupAccessRules.Remove(access);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}