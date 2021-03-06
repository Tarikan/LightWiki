using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Groups.Requests;

public class RemoveGroupPersonalAccessRule : IRequest<OneOf<Success, Fail>>
{
    public int UserId { get; set; }

    public int GroupId { get; set; }
}