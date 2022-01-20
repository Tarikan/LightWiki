using System.Threading.Tasks;
using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Features.Workspaces.Requests;
using LightWiki.Features.Workspaces.Responses.Models;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Web.Extensions;
using MediatR;
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

    [HttpGet]
    [ProducesResponseType(typeof(CollectionResult<WorkspaceModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWorkspaces([FromQuery] GetWorkspaces request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [HttpGet("tree")]
    [ProducesResponseType(typeof(CollectionResult<ArticleHeaderModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTree([FromQuery] GetWorkspaceTree request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

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

    [HttpPost]
    [ProducesResponseType(typeof(SuccessWithId<int>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateWorkspace([FromBody] CreateWorkspace request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }
}