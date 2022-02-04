using System.IO;
using System.Threading.Tasks;

namespace LightWiki.Infrastructure.Aws.S3;

public interface IAwsS3Helper
{
    Task<string> UploadFileToBucket(Stream fileStream, string fileNameWithPath, string contentType);

    public Task DeleteFile(string fileNameWithPath);

    Task DeleteFileByUrl(string url);

#pragma warning disable CA1055
    public string ConstructFileUrl(string fileName);

    public string DeconstructFileUrl(string url);
#pragma warning restore CA1055
}