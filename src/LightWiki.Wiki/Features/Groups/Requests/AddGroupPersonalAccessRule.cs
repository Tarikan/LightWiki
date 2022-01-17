using LightWiki.Domain.Enums;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Groups.Requests;

public class AddGroupPersonalAccessRule : IRequest<OneOf<Success, Fail>>
{
    public int UserId { get; set; }

    public int GroupId { get; set; }

    public GroupAccessRule GroupAccessRule { get; set; }
}