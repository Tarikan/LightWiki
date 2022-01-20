using System.Threading.Tasks;
using LightWiki.Features.Workspaces.Requests;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Web.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LightWiki.Wiki.Api.Controllers;

[ApiController]
[Route("workspace-access")]
public class WorkspaceAccessController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkspaceAccessController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("add-persona")]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    public async Task<IActionResult> GivePersonalAccess([FromBody] AddWorkspacePersonalAccess request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [HttpPost("add-group")]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    public async Task<IActionResult> GiveGroupAccess([FromBody] AddWorkspaceGroupAccess request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [HttpPost("remove-personal")]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemovePersonalAccess([FromBody] RemoveWorkspacePersonalAccess request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [HttpPost("remove-group")]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveGroupAccess([FromBody] RemoveWorkspaceGroupAccess request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }
}