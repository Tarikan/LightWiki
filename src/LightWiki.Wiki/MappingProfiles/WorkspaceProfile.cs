﻿using AutoMapper;
using LightWiki.Domain.Models;
using LightWiki.Features.Workspaces.Requests;
using LightWiki.Features.Workspaces.Responses.Models;

namespace LightWiki.MappingProfiles;

public class WorkspaceProfile : Profile
{
    public WorkspaceProfile()
    {
        CreateMap<CreateWorkspace, Workspace>()
            .ForMember(dest => dest.Id, opts => opts.Ignore())
            .ForMember(dest => dest.Articles, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedAt, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedAt, opts => opts.Ignore())
            .ForMember(dest => dest.UpdatedAt, opts => opts.Ignore())
            .ForMember(dest => dest.GroupAccessRules, opts => opts.Ignore())
            .ForMember(dest => dest.PersonalAccessRules, opts => opts.Ignore());

        CreateMap<Workspace, WorkspaceModel>();
    }
}