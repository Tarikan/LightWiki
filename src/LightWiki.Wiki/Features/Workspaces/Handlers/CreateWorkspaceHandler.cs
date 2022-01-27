using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LightWiki.Data;
using LightWiki.Data.Mongo.Models;
using LightWiki.Data.Mongo.Repositories;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using LightWiki.Features.Articles.Responses.Models;
using LightWiki.Features.Workspaces.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;
using Slugify;

namespace LightWiki.Features.Workspaces.Handlers;

public class CreateWorkspaceHandler : IRequestHandler<CreateWorkspace, OneOf<SuccessWithId<int>, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly IMapper _mapper;
    private readonly IAuthorizedUserProvider _authorizedUserProvider;
    private readonly ISlugHelper _slugHelper;

    public CreateWorkspaceHandler(
        WikiContext wikiContext,
        IMapper mapper,
        IAuthorizedUserProvider authorizedUserProvider,
        ISlugHelper slugHelper)
    {
        _wikiContext = wikiContext;
        _mapper = mapper;
        _authorizedUserProvider = authorizedUserProvider;
        _slugHelper = slugHelper;
    }

    public async Task<OneOf<SuccessWithId<int>, Fail>> Handle(
        CreateWorkspace request,
        CancellationToken cancellationToken)
    {
        var userContext = await _authorizedUserProvider.GetUser();
        var workspace = _mapper.Map<Workspace>(request);
        workspace.Slug = _slugHelper.GenerateSlug(request.Name);

        _wikiContext.Workspaces.Add(workspace);

        await _wikiContext.SaveChangesAsync(cancellationToken);

        var workspaceRule = new WorkspacePersonalAccessRule
        {
            UserId = userContext.Id,
            WorkspaceId = workspace.Id,
            WorkspaceAccessRule = WorkspaceAccessRule.All,
        };
        workspace.PersonalAccessRules ??= new List<WorkspacePersonalAccessRule>();
        workspace.PersonalAccessRules.Add(workspaceRule);
        _wikiContext.Workspaces.Update(workspace);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        var rootArticle = new Article
        {
            Name = workspace.Name,
            Slug = workspace.Slug,
            UserId = userContext.Id,
            WorkspaceId = workspace.Id,
            GlobalAccessRule = ArticleAccessRule.Read,
            PersonalAccessRules = new List<ArticlePersonalAccessRule>
            {
                new ()
                {
                    UserId = userContext.Id,
                    ArticleAccessRule = ArticleAccessRule.All,
                },
            },
        };

        workspace.RootArticle = rootArticle;

        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new SuccessWithId<int>(workspace.Id);
    }
}