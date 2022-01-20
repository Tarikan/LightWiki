using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Workspaces.Requests;

public class GetWorkspaceTree : IRequest<OneOf<CollectionResult<ArticleHeaderModel>, Fail>>
{
    public int WorkspaceId { get; set; }

    /// <summary>
    /// Id of parent node to get next row
    /// </summary>
    public int? ParentArticleId { get; set; }
}