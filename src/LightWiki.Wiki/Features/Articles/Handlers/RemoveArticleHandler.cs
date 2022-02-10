using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Data.Mongo.Repositories;
using LightWiki.Domain.Enums;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Aws.S3;
using LightWiki.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace LightWiki.Features.Articles.Handlers;

public class RemoveArticleHandler : IRequestHandler<RemoveArticle, OneOf<Success, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly IArticleHierarchyNodeRepository _articleHierarchyNodeRepository;
    private readonly IAwsS3Helper _s3Helper;

    public RemoveArticleHandler(
        WikiContext wikiContext,
        IArticleHierarchyNodeRepository articleHierarchyNodeRepository,
        IAwsS3Helper s3Helper)
    {
        _wikiContext = wikiContext;
        _articleHierarchyNodeRepository = articleHierarchyNodeRepository;
        _s3Helper = s3Helper;
    }

    public async Task<OneOf<Success, Fail>> Handle(RemoveArticle request, CancellationToken cancellationToken)
    {
        var article = await _wikiContext.Articles.FindAsync(request.ArticleId);

        var node = await _articleHierarchyNodeRepository.FindByArticleId(article.Id);

        var childNodes = await _articleHierarchyNodeRepository.GetAncestorModels(article.Id);

        _wikiContext.Articles.Remove(article);

        var idsToDelete = childNodes.Select(n => n.Id).ToList();
        idsToDelete.Add(node.Id);

        await _articleHierarchyNodeRepository.RemoveMany(idsToDelete);

        var images = await _wikiContext.Images
            .Where(i => i.OwnerType == OwnerType.Article &&
                        i.OwnerId == request.ArticleId)
            .ToListAsync(cancellationToken);

        await _s3Helper.BatchDelete(images.Select(i => i.Folder));

        _wikiContext.Images.RemoveRange(images);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}