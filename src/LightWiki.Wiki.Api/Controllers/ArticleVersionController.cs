using System.Threading.Tasks;
using LightWiki.Domain.Models;
using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Features.ArticleVersions.Requests;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Web.Authentication;
using LightWiki.Infrastructure.Web.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LightWiki.Wiki.Api.Controllers;

[ConfigurableAuthorize]
[ApiController]
[Route("article-versions")]
public class ArticleVersionController : ControllerBase
{
    private readonly IMediator _mediator;

    public ArticleVersionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(CollectionResult<ArticleVersion>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVersions([FromQuery] GetArticleVersions request)
    {
        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }

    [HttpGet("{id:int}/content")]
    [ProducesResponseType(typeof(ArticleContentModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetContent(int id)
    {
        var request = new GetArticleVersionContent
        {
            ArticleVersionId = id,
        };

        var result = await _mediator.Send(request);

        return result.Match(
            Ok,
            fail => fail.ToActionResult());
    }
}