using LightWiki.Features.Workspaces.Responses.Models;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;
using Sieve.Models;

namespace LightWiki.Features.Workspaces.Requests;

public class GetWorkspaces : SieveModel, IRequest<OneOf<CollectionResult<WorkspaceModel>, Fail>>
{
}