using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LightWiki.Data;
using LightWiki.Data.Mongo.Models;
using LightWiki.Data.Mongo.Repositories;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Extensions;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Articles.Handlers;

public sealed class CreateArticleHandler : IRequestHandler<CreateArticle, OneOf<SuccessWithId<int>, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly IMapper _mapper;
    private readonly IAuthorizedUserProvider _authorizedUserProvider;
    private readonly IWorkspaceTreeRepository _workspaceTreeRepository;

    public CreateArticleHandler(
        WikiContext wikiContext,
        IMapper mapper,
        IAuthorizedUserProvider authorizedUserProvider,
        IWorkspaceTreeRepository workspaceTreeRepository)
    {
        _wikiContext = wikiContext;
        _mapper = mapper;
        _authorizedUserProvider = authorizedUserProvider;
        _workspaceTreeRepository = workspaceTreeRepository;
    }

    public async Task<OneOf<SuccessWithId<int>, Fail>> Handle(
        CreateArticle request,
        CancellationToken cancellationToken)
    {
        var workspace = await _wikiContext.Workspaces.FindAsync(request.WorkspaceId);
        var workspaceTree = await _workspaceTreeRepository.Get(workspace.ArticleTreeId);

        var userContext = await _authorizedUserProvider.GetUser();
        var article = _mapper.Map<Article>(request);

        article.Name = article.Name.ToUrlFriendlyString();
        article.UserId = userContext.Id;
        article.Versions = new List<ArticleVersion>();

        var version = new ArticleVersion()
        {
            Patch = string.Empty,
            Article = article,
            UserId = userContext.Id,
        };

        article.Versions.Add(version);

        await _wikiContext.AddAsync(version, cancellationToken);
        await _wikiContext.Articles.AddAsync(article, cancellationToken);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        var personalAccess = new ArticlePersonalAccessRule
        {
            UserId = userContext.Id,
            ArticleId = article.Id,
            ArticleAccessRule = ArticleAccessRule.All,
        };

        await _wikiContext.ArticlePersonalAccessRules.AddAsync(personalAccess, cancellationToken);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        var articleTreeNode = workspaceTree.ArticleTree.FirstOrDefault();

        if (articleTreeNode is null)
        {
            workspaceTree.ArticleTree.Add(new ArticleTreeNode
            {
                ArticleId = article.Id,
                Name = article.Name,
                Children = new List<ArticleTreeNode>(),
            });
        }
        else
        {
            for (var i = 1; i < request.ParentIds.Count; i++)
            {
                articleTreeNode =
                    articleTreeNode.Children.SingleOrDefault(n => n.ArticleId == request.ParentIds[i]);

                if (articleTreeNode is null)
                {
                    return new Fail("Wrong parent path", FailCode.BadRequest);
                }
            }

            articleTreeNode.Children.Add(new ArticleTreeNode
            {
                ArticleId = article.Id,
                Name = article.Name,
                Children = new List<ArticleTreeNode>(),
            });
        }

        var treeId = await _workspaceTreeRepository.Update(workspaceTree.Id, workspaceTree);

        workspace.ArticleTreeId = treeId;
        _wikiContext.Workspaces.Update(workspace);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new SuccessWithId<int>(article.Id);
    }
}