using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Data.Mongo.Repositories;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Articles.Handlers;

public class RemoveArticleHandler : IRequestHandler<RemoveArticle, OneOf<Success, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly IArticleHierarchyNodeRepository _articleHierarchyNodeRepository;

    public RemoveArticleHandler(WikiContext wikiContext, IArticleHierarchyNodeRepository articleHierarchyNodeRepository)
    {
        _wikiContext = wikiContext;
        _articleHierarchyNodeRepository = articleHierarchyNodeRepository;
    }

    public async Task<OneOf<Success, Fail>> Handle(RemoveArticle request, CancellationToken cancellationToken)
    {
        var article = await _wikiContext.Articles.FindAsync(request.ArticleId);

        var node = await _articleHierarchyNodeRepository.FindByArticleId(article.Id);

        var childNodes = await _articleHierarchyNodeRepository.GetAncestorModels(article.Id);

        _wikiContext.Articles.Remove(article);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        var idsToDelete = childNodes.Select(n => n.Id).ToList();
        idsToDelete.Add(node.Id);

        await _articleHierarchyNodeRepository.RemoveMany(idsToDelete);

        return new Success();
    }
}