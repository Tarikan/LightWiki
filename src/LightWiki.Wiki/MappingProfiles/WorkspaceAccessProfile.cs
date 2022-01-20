using AutoMapper;
using LightWiki.Domain.Models;
using LightWiki.Features.Workspaces.Responses.Models;

namespace LightWiki.MappingProfiles;

public class WorkspaceAccessProfile : Profile
{
    public WorkspaceAccessProfile()
    {
        CreateMap<WorkspacePersonalAccessRule, WorkspacePersonalAccessRuleModel>();
        CreateMap<WorkspaceGroupAccessRule, WorkspaceGroupAccessRuleModel>();
    }
}