using System.Threading.Tasks;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Web.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LightWiki.Wiki.Api.Controllers;

[Authorize]
[ApiController]
[Route("article-access")]
public class ArticleAccessController : ControllerBase
{
    private readonly IMediator _mediator;

    public ArticleAccessController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("add-personal")]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddPersonalAccess([FromBody] AddPersonalAccess request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [HttpPost("add-group")]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddGroupAccess([FromBody] AddGroupAccess request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [HttpPost("remove-personal")]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemovePersonalAccess([FromBody] RemovePersonalAccess request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [HttpPost("remove-group")]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveGroupAccess([FromBody] RemoveGroupAccess request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }
}