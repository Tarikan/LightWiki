using System.IO;
using System.Threading.Tasks;
using LightWiki.Features.Users.Requests;
using LightWiki.Features.Users.Responses.Models;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Web.Authentication;
using LightWiki.Infrastructure.Web.Extensions;
using LightWiki.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LightWiki.Wiki.Api.Controllers;

[ConfigurableAuthorize]
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

    [HttpPost("avatar")]
    [ProducesResponseType(typeof(ResponsiveImageModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        var stream = new MemoryStream();
        await file.OpenReadStream().CopyToAsync(stream);
        stream.Position = 0;
        var request = new UploadUserImage
        {
            Image = stream,
            ContentType = file.ContentType,
        };

        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [Authorize]
    [HttpPut("info")]
    [ProducesResponseType(typeof(Success), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserInfo request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Accepted,
            fail => fail.ToActionResult());
    }
}