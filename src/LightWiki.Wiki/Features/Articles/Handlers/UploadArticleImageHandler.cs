using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Aws.S3;
using LightWiki.Infrastructure.Configuration;
using LightWiki.Infrastructure.Models;
using LightWiki.Shared.Helpers;
using LightWiki.Shared.Models;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OneOf;

namespace LightWiki.Features.Articles.Handlers;

public class UploadArticleImageHandler : IRequestHandler<UploadArticleImage, OneOf<ResponsiveImageModel, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly IAwsS3Helper _s3Helper;
    private readonly IImageHelper _imageHelper;
    private readonly IHashHelper _hashHelper;
    private readonly ImageSizeConfiguration _imageSizeConfiguration;

    public UploadArticleImageHandler(
        WikiContext wikiContext,
        IAwsS3Helper s3Helper,
        IImageHelper imageHelper,
        IHashHelper hashHelper,
        ImageSizeConfiguration imageSizeConfiguration)
    {
        _wikiContext = wikiContext;
        _s3Helper = s3Helper;
        _imageHelper = imageHelper;
        _hashHelper = hashHelper;
        _imageSizeConfiguration = imageSizeConfiguration;
    }

    public async Task<OneOf<ResponsiveImageModel, Fail>> Handle(
        UploadArticleImage request,
        CancellationToken cancellationToken)
    {
        request.Image.Position = 0;
        var bytes = new byte[request.Image.Length];
        await request.Image.ReadAsync(bytes.AsMemory(0, Convert.ToInt32(request.Image.Length)), cancellationToken);

        var hash = _hashHelper.GetMd5Hash(bytes);

        var sizes = new int[]
        {
            _imageSizeConfiguration.Small,
            _imageSizeConfiguration.Medium,
            _imageSizeConfiguration.Large,
            _imageSizeConfiguration.ExtraLarge,
        };

        var tasks = sizes.Select(
                s => Task.Run(
                    () => Task.FromResult(_imageHelper.ResizeImage(bytes, s)),
                    cancellationToken))
            .ToList();

        var fileName = $"articles/{request.ArticleId}/images/{hash}/img";

        request.Image.Position = 0;
        await _s3Helper.UploadFileToBucket(
            request.Image,
            fileName,
            request.ContentType);

        for (var i = 0; i < sizes.Length; i++)
        {
            var stream = await tasks[i];
            await _s3Helper.UploadFileToBucket(
                stream,
                fileName + $"-{sizes[i]}",
                "image/png");
        }

        var metadata = new ImageMetadata
        {
            AvailableSizes = sizes.ToList(),
        };
        var jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        };

        var image = new Image
        {
            OwnerId = request.ArticleId,
            OwnerType = OwnerType.Article,
            Folder = $"articles/{request.ArticleId}/images/{hash}",
            FileName = "img",
            Metadata = JsonConvert.SerializeObject(metadata, Formatting.None, jsonSerializerSettings),
        };

        _wikiContext.Images.Add(image);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        var urls = new Dictionary<string, string>
        {
            { "default", _s3Helper.ConstructFileUrl(fileName) },
        };

        foreach (var size in sizes)
        {
            urls.Add(size.ToString(), _s3Helper.ConstructFileUrl(fileName + $"-{size}"));
        }

        return new ResponsiveImageModel
        {
            Urls = urls,
        };
    }
}