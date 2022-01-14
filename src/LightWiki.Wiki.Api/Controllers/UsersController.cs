using System.Threading.Tasks;
using LightWiki.Features.Users.Requests;
using LightWiki.Features.Users.Responses.Models;
using LightWiki.Infrastructure.Web.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LightWiki.Wiki.Api.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUser(int id)
    {
        var request = new GetUser
        {
            UserId = id,
        };

        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }
}