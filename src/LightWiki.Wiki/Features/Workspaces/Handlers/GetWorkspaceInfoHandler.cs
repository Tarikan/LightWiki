using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LightWiki.Data;
using LightWiki.Features.Workspaces.Requests;
using LightWiki.Features.Workspaces.Responses.Models;
using LightWiki.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace LightWiki.Features.Workspaces.Handlers;

public class GetWorkspaceInfoHandler : IRequestHandler<GetWorkspaceInfo, OneOf<WorkspaceInfoModel, Fail>>
{
    private readonly WikiContext _wikiContext;
    private readonly IMapper _mapper;

    public GetWorkspaceInfoHandler(WikiContext wikiContext, IMapper mapper)
    {
        _wikiContext = wikiContext;
        _mapper = mapper;
    }

    public async Task<OneOf<WorkspaceInfoModel, Fail>> Handle(GetWorkspaceInfo request, CancellationToken cancellationToken)
    {
        var workspace = await _wikiContext.Workspaces
            .Include(w => w.GroupAccessRules)
            .Include(w => w.PersonalAccessRules)
            .SingleAsync(
                w => w.Id == request.WorkspaceId,
                cancellationToken);

        return new WorkspaceInfoModel
        {
            Id = workspace.Id,
            PersonalRules = _mapper.Map<List<WorkspacePersonalAccessRuleModel>>(workspace.PersonalAccessRules),
            GroupRules = _mapper.Map<List<WorkspaceGroupAccessRuleModel>>(workspace.GroupAccessRules),
        };
    }
}