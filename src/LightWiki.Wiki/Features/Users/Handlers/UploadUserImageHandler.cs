using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using LightWiki.Features.Users.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Aws.S3;
using LightWiki.Infrastructure.Configuration;
using LightWiki.Infrastructure.Models;
using LightWiki.Shared.Helpers;
using LightWiki.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OneOf;

namespace LightWiki.Features.Users.Handlers;

public class UploadUserImageHandler : IRequestHandler<UploadUserImage, OneOf<ResponsiveImageModel, Fail>>
{
    private readonly IImageHelper _imageHelper;
    private readonly WikiContext _wikiContext;
    private readonly IAwsS3Helper _s3Helper;
    private readonly ImageSizeConfiguration _imageSizeConfiguration;
    private readonly IAuthorizedUserProvider _authorizedUserProvider;

    public UploadUserImageHandler(
        IImageHelper imageHelper,
        WikiContext wikiContext,
        IAwsS3Helper s3Helper,
        ImageSizeConfiguration imageSizeConfiguration,
        IAuthorizedUserProvider authorizedUserProvider)
    {
        _imageHelper = imageHelper;
        _wikiContext = wikiContext;
        _s3Helper = s3Helper;
        _imageSizeConfiguration = imageSizeConfiguration;
        _authorizedUserProvider = authorizedUserProvider;
    }

    public async Task<OneOf<ResponsiveImageModel, Fail>> Handle(
        UploadUserImage request,
        CancellationToken cancellationToken)
    {
        var userContext = await _authorizedUserProvider.GetUser();

        var prevImage = await _wikiContext.Images
            .SingleOrDefaultAsync(
                i => i.OwnerType == OwnerType.User &&
                     i.OwnerId == userContext.Id,
                cancellationToken);

        if (prevImage != null)
        {
            await _s3Helper.DeleteFile(prevImage.Folder);
            _wikiContext.Images.Remove(prevImage);
        }

        request.Image.Position = 0;
        var bytes = new byte[request.Image.Length];
        await request.Image.ReadAsync(bytes.AsMemory(0, Convert.ToInt32(request.Image.Length)), cancellationToken);

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

        var fileName = $"users/{userContext.Id}/avatar/avatar";

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
            OwnerId = userContext.Id,
            OwnerType = OwnerType.User,
            Folder = $"users/{userContext.Id}/avatar",
            FileName = "avatar",
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