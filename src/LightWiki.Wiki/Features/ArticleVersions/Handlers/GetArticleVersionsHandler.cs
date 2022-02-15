using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Features.ArticleVersions.Requests;
using LightWiki.Features.ArticleVersions.Responses.Models;
using LightWiki.Infrastructure.Aws.S3;
using LightWiki.Infrastructure.Models;
using LightWiki.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using Sieve.Services;

namespace LightWiki.Features.ArticleVersions.Handlers;

public class
    GetArticleVersionsHandler : IRequestHandler<GetArticleVersions, OneOf<CollectionResult<ArticleVersionModel>, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly ISieveProcessor _sieveProcessor;
    private readonly IMapper _mapper;
    private readonly IAwsS3Helper _s3Helper;

    public GetArticleVersionsHandler(
        WikiContext wikiContext,
        ISieveProcessor sieveProcessor,
        IMapper mapper,
        IAwsS3Helper s3Helper)
    {
        _wikiContext = wikiContext;
        _sieveProcessor = sieveProcessor;
        _mapper = mapper;
        _s3Helper = s3Helper;
    }

    public async Task<OneOf<CollectionResult<ArticleVersionModel>, Fail>> Handle(
        GetArticleVersions request,
        CancellationToken cancellationToken)
    {
        var versions = _wikiContext.ArticleVersions
            .Include(v => v.User)
            .Where(v => v.ArticleId == request.ArticleId)
            .OrderByDescending(v => v.CreatedAt)
            .AsNoTracking();

        var total = await versions.CountAsync(cancellationToken);

        var queryResult = await _sieveProcessor.Apply(request, versions)
            .ToListAsync(cancellationToken);

        var userIds = queryResult.Select(qr => qr.User.Id);
        var userImages = await _wikiContext.Images.Where(i => i.OwnerType == OwnerType.User &&
                                                              userIds.Contains(i.OwnerId))
            .ToListAsync(cancellationToken);

        var models = _mapper.Map<List<ArticleVersionModel>>(queryResult);

        foreach (var articleVersionModel in models)
        {
            var avatar = userImages.SingleOrDefault(ui => ui.OwnerId == articleVersionModel.UserId);
            if (avatar != null)
            {
                articleVersionModel.User.Avatar = new ImageModel
                {
                    FileUrl = _s3Helper.ConstructFileUrl(avatar.Folder + '/' + avatar.FileName),
                    ImageMetadata = avatar.Metadata,
                };
            }
        }

        return new CollectionResult<ArticleVersionModel>(models, total);
    }
}