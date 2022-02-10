using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using LightWiki.Infrastructure.Configuration.Aws;

namespace LightWiki.Infrastructure.Aws.S3;

public class AwsS3Helper : IAwsS3Helper
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3Configuration _s3Configuration;

    public AwsS3Helper(IAmazonS3 s3Client, S3Configuration s3Configuration)
    {
        _s3Client = s3Client;
        _s3Configuration = s3Configuration;
    }

    public async Task<string> UploadFileToBucket(Stream fileStream, string fileNameWithPath, string contentType)
    {
        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = fileStream,
            Key = fileNameWithPath,
            BucketName = _s3Configuration.BucketName,
            ContentType = contentType,
        };

        using var transferUtility = new TransferUtility(_s3Client);
        await transferUtility.UploadAsync(uploadRequest);

        var fileUrl = ConstructFileUrl(fileNameWithPath);
        return fileUrl;
    }

    public async Task DeleteFile(string fileNameWithPath)
    {
        var deleteObjectRequest = new DeleteObjectRequest
        {
            BucketName = _s3Configuration.BucketName,
            Key = fileNameWithPath,
        };

        await _s3Client.DeleteObjectAsync(deleteObjectRequest);
    }

    public async Task BatchDelete(IEnumerable<string> fileNames)
    {
        var keyVersions = fileNames.Select(n => new KeyVersion
        {
            Key = n,
        })
            .ToList();

        var deleteRequest = new DeleteObjectsRequest
        {
            BucketName = _s3Configuration.BucketName,
            Objects = keyVersions,
        };

        await _s3Client.DeleteObjectsAsync(deleteRequest);
    }

    public async Task DeleteFileByUrl(string url)
    {
        var fileNameWithPath = DeconstructFileUrl(url);
        var deleteObjectRequest = new DeleteObjectRequest
        {
            BucketName = _s3Configuration.BucketName,
            Key = fileNameWithPath,
        };
        await _s3Client.DeleteObjectAsync(deleteObjectRequest);
    }

#pragma warning disable CA1055
    public string ConstructFileUrl(string fileName)
    {
        var fileUrl = $"https://{_s3Configuration.BucketName}.s3.{_s3Client.Config.RegionEndpoint.SystemName}.amazonaws.com/{fileName}";
        return fileUrl;
    }

    public string DeconstructFileUrl(string url)
    {
        var awsS3Url = new AmazonS3Uri(url);
        return awsS3Url.Key;
    }
#pragma warning restore CA1055
}