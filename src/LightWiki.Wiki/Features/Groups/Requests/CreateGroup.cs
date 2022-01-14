using LightWiki.Domain.Enums;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Groups.Requests;

public class CreateGroup : IRequest<OneOf<SuccessWithId<int>, Fail>>
{
    public string GroupName { get; set; }

    public GroupAccessRule GroupAccessRule { get; set; }
}