using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightWiki.ArticleEngine.Patches;
using LightWiki.Data;
using LightWiki.Data.Mongo.Repositories;
using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Features.ArticleVersions.Requests;
using LightWiki.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace LightWiki.Features.ArticleVersions.Handlers;

public class
    GetArticleVersionContentHandler : IRequestHandler<GetArticleVersionContent, OneOf<ArticleContentModel, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly IPatchHelper _patchHelper;

    public GetArticleVersionContentHandler(WikiContext wikiContext, IPatchHelper patchHelper)
    {
        _wikiContext = wikiContext;
        _patchHelper = patchHelper;
    }

    public async Task<OneOf<ArticleContentModel, Fail>> Handle(
        GetArticleVersionContent request,
        CancellationToken cancellationToken)
    {
        var articleVersion = await _wikiContext.ArticleVersions.FindAsync(request.ArticleVersionId);

        var patches = await _wikiContext.ArticleVersions
            .Where(v => v.ArticleId == articleVersion.ArticleId &&
                        v.CreatedAt < articleVersion.CreatedAt)
            .OrderBy(v => v.CreatedAt)
            .Select(v => v.Patch)
            .ToListAsync(cancellationToken);

        patches.Add(articleVersion.Patch);

        var text = _patchHelper.ApplyPatches(patches, string.Empty);

        return new ArticleContentModel
        {
            Text = text,
        };
    }
}