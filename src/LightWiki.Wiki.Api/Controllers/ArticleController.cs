using System.Threading.Tasks;
using LightWiki.Features.Articles.Requests;
using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Web.Authentication;
using LightWiki.Infrastructure.Web.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LightWiki.Wiki.Api.Controllers;

[ApiController]
[Route("articles")]
public class ArticleController : ControllerBase
{
    private readonly IMediator _mediator;

    public ArticleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [ConfigurableAuthorize]
    [ProducesResponseType(typeof(ArticleModel), StatusCodes.Status200OK)]
    [HttpGet("display/{workspaceSlug}/{articleNameSlug}")]
    public async Task<IActionResult> SearchBySlag(string workspaceSlug, string articleNameSlug)
    {
        var request = new GetArticleBySlug
        {
            ArticleNameSlug = articleNameSlug,
            WorkspaceNameSlug = workspaceSlug,
        };

        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [ConfigurableAuthorize]
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ArticleModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetArticle(int id)
    {
        var request = new GetArticle
        {
            ArticleId = id,
        };

        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [ConfigurableAuthorize]
    [HttpGet]
    [ProducesResponseType(typeof(CollectionResult<ArticleModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetArticles([FromQuery] GetArticles request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [ConfigurableAuthorize]
    [HttpGet("{id:int}/content")]
    [ProducesResponseType(typeof(ArticleContentModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetArticleContent(int id)
    {
        var request = new GetArticleContent
        {
            ArticleId = id,
        };

        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateArticle([FromBody] CreateArticle request)
    {
        request.Name = request.Name.Trim();
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [Authorize]
    [HttpPost("{id:int}/content")]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateArticleContent(int id, [FromBody] UpdateArticleContent request)
    {
        request.ArticleId = id;

        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [Authorize]
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateArticle(int id, [FromBody] UpdateArticle request)
    {
        request.Name = request.Name.Trim();
        request.Id = id;

        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveArticle(int id)
    {
        var request = new RemoveArticle
        {
            ArticleId = id,
        };
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [ConfigurableAuthorize]
    [HttpGet("{id:int}/ancestors")]
    [ProducesResponseType(typeof(ArticleAncestorsModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAncestors(int id)
    {
        var request = new GetArticleAncestors
        {
            ArticleId = id,
        };
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }
}