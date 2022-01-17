using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace LightWiki.Features.Articles.Handlers;

public class AddGroupAccessHandler : IRequestHandler<AddGroupAccess, OneOf<Success, Fail>>
{
    private readonly WikiContext _wikiContext;

    public AddGroupAccessHandler(WikiContext wikiContext)
    {
        _wikiContext = wikiContext;
    }

    public async Task<OneOf<Success, Fail>> Handle(AddGroupAccess request, CancellationToken cancellationToken)
    {
        var access = await _wikiContext.ArticleGroupAccessRules
            .SingleOrDefaultAsync(
                r => r.GroupId == request.GroupId &&
                     r.ArticleId == request.ArticleId,
                cancellationToken);

        access ??= new ArticleGroupAccessRule
        {
            GroupId = request.GroupId,
            ArticleId = request.ArticleId,
        };

        access.ArticleAccessRule = request.AccessRule;

        _wikiContext.ArticleGroupAccessRules.Update(access);

        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}