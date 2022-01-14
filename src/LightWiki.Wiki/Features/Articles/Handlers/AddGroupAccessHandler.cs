using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Models;
using MediatR;
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
        var access = new ArticleGroupAccessRule
        {
            GroupId = request.GroupId,
            ArticleId = request.ArticleId,
            ArticleAccessRule = request.AccessRule,
        };

        await _wikiContext.ArticleGroupAccessRules.AddAsync(access, cancellationToken);

        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}