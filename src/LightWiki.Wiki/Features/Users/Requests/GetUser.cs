using LightWiki.Features.Users.Responses.Models;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Users.Requests;

public class GetUser : IRequest<OneOf<UserModel, Fail>>
{
    public int UserId { get; set; }
}