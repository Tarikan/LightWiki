using System.Threading.Tasks;
using LightWiki.Features.Groups.Requests;
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
public class GroupAccessController : ControllerBase
{
    private readonly IMediator _mediator;

    public GroupAccessController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("add-access")]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddPersonalAccess([FromBody] AddGroupPersonalAccessRule request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [HttpPost("remove-access")]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemovePersonalAccess([FromBody] RemoveGroupPersonalAccessRule request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }
}