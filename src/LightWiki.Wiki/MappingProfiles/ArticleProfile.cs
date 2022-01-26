using AutoMapper;
using LightWiki.Domain.Models;
using LightWiki.Features.Articles.Requests;
using LightWiki.Features.Articles.Requests.Models;
using LightWiki.Features.Articles.Responses.Models;

namespace LightWiki.MappingProfiles;

public class ArticleProfile : Profile
{
    public ArticleProfile()
    {
        CreateMap<Article, ArticleModel>()
            .ForMember(dest => dest.GlobalAccessRule, opts => opts.MapFrom(src => new RequestAccessModel(src.GlobalAccessRule)));

        CreateMap<UpdateArticle, Article>()
            .ForMember(dest => dest.Id, opts => opts.Ignore())
            .ForMember(dest => dest.User, opts => opts.Ignore())
            .ForMember(dest => dest.UserId, opts => opts.Ignore())
            .ForMember(dest => dest.Versions, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedAt, opts => opts.Ignore())
            .ForMember(dest => dest.UpdatedAt, opts => opts.Ignore())
            .ForMember(dest => dest.GroupAccessRules, opts => opts.Ignore())
            .ForMember(dest => dest.PersonalAccessRules, opts => opts.Ignore())
            .ForMember(dest => dest.Workspace, opts => opts.Ignore())
            .ForMember(dest => dest.WorkspaceId, opts => opts.Ignore())
            .ForMember(dest => dest.ParentArticle, opts => opts.Ignore())
            .ForMember(dest => dest.ParentArticleId, opts => opts.Ignore())
            .ForMember(dest => dest.Slug, opts => opts.Ignore());

        CreateMap<CreateArticle, Article>()
            .ForMember(dest => dest.Id, opts => opts.Ignore())
            .ForMember(dest => dest.User, opts => opts.Ignore())
            .ForMember(dest => dest.UserId, opts => opts.Ignore())
            .ForMember(dest => dest.Versions, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedAt, opts => opts.Ignore())
            .ForMember(dest => dest.UpdatedAt, opts => opts.Ignore())
            .ForMember(dest => dest.GroupAccessRules, opts => opts.Ignore())
            .ForMember(dest => dest.PersonalAccessRules, opts => opts.Ignore())
            .ForMember(dest => dest.Workspace, opts => opts.Ignore())
            .ForMember(dest => dest.ParentArticle, opts => opts.Ignore())
            .ForMember(dest => dest.Slug, opts => opts.Ignore())
            .ForMember(dest => dest.ParentArticleId, opts => opts.MapFrom(src => src.ParentId));

        CreateMap<Article, ArticleHeaderModel>();
    }
}