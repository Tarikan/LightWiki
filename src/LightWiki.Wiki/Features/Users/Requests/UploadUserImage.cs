using System.Collections.Generic;
using System.IO;
using LightWiki.Infrastructure.Models;
using LightWiki.Shared.Models;
using MediatR;
using Newtonsoft.Json;
using OneOf;

namespace LightWiki.Features.Users.Requests;

public class UploadUserImage : IRequest<OneOf<ResponsiveImageModel, Fail>>
{
    [JsonIgnore] public static readonly List<string> AcceptedFileMimeTypes = new List<string>
    {
        "image/png",
        "image/jpeg",
    };

    public Stream Image { get; set; }

    public string ContentType { get; set; }
}