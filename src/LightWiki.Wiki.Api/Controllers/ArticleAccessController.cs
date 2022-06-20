using System.Threading.Tasks;
using LightWiki.Features.Articles.Requests;
using LightWiki.Features.Articles.Responses.Models;
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

    [HttpPost]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddPersonalAccess([FromBody] AddArticleAccess request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [HttpDelete]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveGroupAccess([FromBody] RemoveArticleAccess request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [HttpGet]
    [Route("{articleId:int}")]
    [ProducesResponseType(typeof(CollectionResult<ArticleAccessRuleModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetArticleAccessRules(int articleId, [FromQuery] GetArticleAccessRules request)
    {
        request.ArticleId = articleId;

        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }
}