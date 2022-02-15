using System.Threading.Tasks;
using LightWiki.Features.Workspaces.Handlers;
using LightWiki.Features.Workspaces.Requests;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Web.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LightWiki.Wiki.Api.Controllers;

[Authorize]
[ApiController]
[Route("workspace-access")]
public class WorkspaceAccessController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkspaceAccessController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    public async Task<IActionResult> GivePersonalAccess([FromBody] AddWorkspaceAccess request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [HttpDelete]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveGroupAccess([FromBody] RemoveWorkspaceAccess request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }
}