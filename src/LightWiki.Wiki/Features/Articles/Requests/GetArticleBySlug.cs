using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Articles.Requests;

public class GetArticleBySlug : IRequest<OneOf<ArticleModel, Fail>>
{
    public string WorkspaceNameSlug { get; set; }

    public string ArticleNameSlug { get; set; }
}