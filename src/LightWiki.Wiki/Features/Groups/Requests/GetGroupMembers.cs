using LightWiki.Features.Users.Responses.Models;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Groups.Requests;

public class GetGroupMembers : IRequest<OneOf<CollectionResult<UserModel>, Fail>>
{
    public int GroupId { get; set; }
}