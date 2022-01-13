using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Groups.Requests;

public class CreateGroup : IRequest<OneOf<Success, Fail>>
{
    public string GroupName { get; set; }
}