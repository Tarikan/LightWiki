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

[ApiController]
[Route("articles")]
public class ArticleController : ControllerBase
{
    private readonly IMediator _mediator;

    public ArticleController(IMediator mediator)
    {
        _mediator = mediator;
    }

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

    [HttpGet]
    [ProducesResponseType(typeof(CollectionResult<ArticleModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetArticles([FromQuery] GetArticles request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

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

    [HttpPost]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateArticle([FromBody] CreateArticle request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

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

    [HttpPut]
    [ProducesResponseType(typeof(Success), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateArticle([FromBody] UpdateArticle request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }
}