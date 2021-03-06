using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Groups.Requests;

public class RemoveGroup : IRequest<OneOf<Success, Fail>>
{
    public int Id { get; set; }
}