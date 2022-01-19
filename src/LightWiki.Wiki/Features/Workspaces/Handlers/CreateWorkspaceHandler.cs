using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LightWiki.Data;
using LightWiki.Data.Mongo.Models;
using LightWiki.Data.Mongo.Repositories;
using LightWiki.Domain.Models;
using LightWiki.Features.Workspaces.Requests;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Workspaces.Handlers;

public class CreateWorkspaceHandler : IRequestHandler<CreateWorkspace, OneOf<SuccessWithId<int>, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly IMapper _mapper;
    private readonly IWorkspaceTreeRepository _workspaceTreeRepository;

    public CreateWorkspaceHandler(
        WikiContext wikiContext,
        IMapper mapper,
        IWorkspaceTreeRepository workspaceTreeRepository)
    {
        _wikiContext = wikiContext;
        _mapper = mapper;
        _workspaceTreeRepository = workspaceTreeRepository;
    }

    public async Task<OneOf<SuccessWithId<int>, Fail>> Handle(
        CreateWorkspace request,
        CancellationToken cancellationToken)
    {
        var workspace = _mapper.Map<Workspace>(request);

        _wikiContext.Workspaces.Add(workspace);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        var workspaceTree = new WorkspaceTree
        {
            WorkspaceName = workspace.Name,
            WorkspaceId = workspace.Id,
            ArticleTree = new List<ArticleTreeNode>(),
        };

        await _workspaceTreeRepository.Create(workspaceTree);
        workspace.ArticleTreeId = workspaceTree.Id;
        _wikiContext.Workspaces.Update(workspace);
        await _wikiContext.SaveChangesAsync(cancellationToken);

        return new SuccessWithId<int>(workspace.Id);
    }
}