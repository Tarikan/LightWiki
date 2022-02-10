using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LightWiki.Infrastructure.Aws.S3;

public interface IAwsS3Helper
{
    Task<string> UploadFileToBucket(Stream fileStream, string fileNameWithPath, string contentType);

    Task DeleteFile(string fileNameWithPath);

    Task BatchDelete(IEnumerable<string> fileNames);

    Task DeleteFileByUrl(string url);

#pragma warning disable CA1055
    string ConstructFileUrl(string fileName);

    string DeconstructFileUrl(string url);
#pragma warning restore CA1055
}