using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Features.Users.Requests;
using LightWiki.Features.Users.Responses.Models;
using LightWiki.Infrastructure.Aws.S3;
using LightWiki.Infrastructure.Models;
using LightWiki.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace LightWiki.Features.Users.Handlers;

public class GetUserHandler : IRequestHandler<GetUser, OneOf<UserModel, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly IMapper _mapper;
    private readonly IAwsS3Helper _awsS3Helper;

    public GetUserHandler(IMapper mapper, WikiContext wikiContext, IAwsS3Helper awsS3Helper)
    {
        _mapper = mapper;
        _wikiContext = wikiContext;
        _awsS3Helper = awsS3Helper;
    }

    public async Task<OneOf<UserModel, Fail>> Handle(GetUser request, CancellationToken cancellationToken)
    {
        var user = await _wikiContext.Users.FindAsync(request.UserId);

        var userModel = _mapper.Map<UserModel>(user);

        var image = await _wikiContext.Images
            .Where(i => i.OwnerType == OwnerType.User &&
                        i.OwnerId == request.UserId)
            .SingleOrDefaultAsync(cancellationToken);

        if (image != null)
        {
            userModel.Avatar = new ImageModel
            {
                FileUrl = _awsS3Helper.ConstructFileUrl(image.Folder + '/' + image.FileName),
                ImageMetadata = image.Metadata,
            };
        }

        return userModel;
    }
}