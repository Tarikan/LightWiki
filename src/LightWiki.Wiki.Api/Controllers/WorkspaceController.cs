using System.Threading.Tasks;
using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Features.Workspaces.Requests;
using LightWiki.Features.Workspaces.Responses.Models;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Web.Authentication;
using LightWiki.Infrastructure.Web.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LightWiki.Wiki.Api.Controllers;

[ApiController]
[Route("workspaces")]
public class WorkspaceController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkspaceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [ConfigurableAuthorize]
    [HttpGet]
    [ProducesResponseType(typeof(CollectionResult<WorkspaceModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWorkspaces([FromQuery] GetWorkspaces request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [ConfigurableAuthorize]
    [HttpGet("tree")]
    [ProducesResponseType(typeof(CollectionResult<ArticleHeaderModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTree([FromQuery] GetWorkspaceTree request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [ConfigurableAuthorize]
    [HttpGet("{id:int}/info")]
    [ProducesResponseType(typeof(WorkspaceInfoModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInfo(int id)
    {
        var request = new GetWorkspaceInfo
        {
            WorkspaceId = id,
        };

        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(SuccessWithId<int>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateWorkspace([FromBody] CreateWorkspace request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [Authorize]
    [HttpPut]
    [ProducesResponseType(typeof(Success), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> UpdateWorkspace([FromBody] UpdateWorkspace request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteWorkspace(int id)
    {
        var request = new RemoveWorkspace
        {
            WorkspaceId = id,
        };

        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }
}