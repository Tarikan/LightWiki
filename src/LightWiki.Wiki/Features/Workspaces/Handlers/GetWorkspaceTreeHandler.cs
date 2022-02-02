using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LightWiki.Data;
using LightWiki.Data.Mongo.Models;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Features.Workspaces.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace LightWiki.Features.Workspaces.Handlers;

public class
    GetWorkspaceTreeHandler : IRequestHandler<GetWorkspaceTree, OneOf<CollectionResult<ArticleHeaderModel>, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly IAuthorizedUserProvider _authorizedUserProvider;
    private readonly IMapper _mapper;

    public GetWorkspaceTreeHandler(
        WikiContext wikiContext,
        IAuthorizedUserProvider authorizedUserProvider,
        IMapper mapper)
    {
        _wikiContext = wikiContext;
        _authorizedUserProvider = authorizedUserProvider;
        _mapper = mapper;
    }

    public async Task<OneOf<CollectionResult<ArticleHeaderModel>, Fail>> Handle(
        GetWorkspaceTree request,
        CancellationToken cancellationToken)
    {
        var userContext = await _authorizedUserProvider.GetUserOrDefault();

        var articlesRequest = _wikiContext.Articles
            .Where(a => (a.ParentArticleId == request.ParentArticleId ||
                         a.ParentArticle.ParentArticleId == request.ParentArticleId) &&
                        a.Id != a.Workspace.RootArticleId &&
                        a.WorkspaceId == request.WorkspaceId)
            .AsNoTracking();

        if (userContext is null)
        {
            articlesRequest = articlesRequest.Where(a => a.GlobalAccessRule.HasFlag(ArticleAccessRule.Read) &&
                                                         a.Workspace.WorkspaceAccessRule.HasFlag(WorkspaceAccessRule
                                                             .Browse));
        }
        else
        {
            articlesRequest = articlesRequest
                .Where(a => a.PersonalAccessRules.Any(par => par.UserId == userContext.Id &&
                                                             par.ArticleAccessRule.HasFlag(ArticleAccessRule.Read)) ||
                            a.PersonalAccessRules.All(par => par.UserId != userContext.Id) &&
                            a.GroupAccessRules.Any(gar => gar.Group.Users.Any(u => u.Id == userContext.Id) &&
                                                          gar.ArticleAccessRule.HasFlag(ArticleAccessRule.Read)) ||
                            a.GlobalAccessRule.HasFlag(ArticleAccessRule.Read) &&
                            a.PersonalAccessRules.All(par => par.UserId != userContext.Id) &&
                            a.GroupAccessRules
                                .Where(gar => gar.Group.Users.Any(u => u.Id == userContext.Id))
                                .All(gar => !gar.ArticleAccessRule.HasFlag(ArticleAccessRule.Read)));
        }

        var result = await articlesRequest.ToListAsync(cancellationToken);
        var ids = result.Select(r => r.Id);

        var hasChildren = await _wikiContext.Articles.Where(a => a.ParentArticleId.HasValue &&
                                                                 ids.Contains(a.ParentArticleId.Value))
            .GroupBy(a => a.ParentArticleId)
            .Where(g => g.Any())
            .Select(g => g.Key)
            .ToListAsync(cancellationToken);

        var mapped =
            _mapper.Map<List<ArticleHeaderModel>>(result);

        foreach (var model in mapped)
        {
            model.HasChildren = hasChildren.Contains(model.Id);
        }

        return new CollectionResult<ArticleHeaderModel>(mapped, mapped.Count);
    }
}