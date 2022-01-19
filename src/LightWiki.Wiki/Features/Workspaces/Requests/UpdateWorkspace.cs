using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Workspaces.Requests;

public class UpdateWorkspace : IRequest<OneOf<Success, Fail>>
{
    public int Id { get; set; }

    public string Name { get; set; }
}