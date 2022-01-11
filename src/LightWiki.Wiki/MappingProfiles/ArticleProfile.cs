using AutoMapper;
using LightWiki.Domain.Models;
using LightWiki.Features.Articles.Requests;
using LightWiki.Features.Articles.Responses.Models;

namespace LightWiki.MappingProfiles;

public class ArticleProfile : Profile
{
    public ArticleProfile()
    {
        CreateMap<Article, ArticleModel>();
        CreateMap<CreateArticle, Article>()
            .ForMember(dest => dest.Id, opts => opts.Ignore())
            .ForMember(dest => dest.User, opts => opts.Ignore())
            .ForMember(dest => dest.UserId, opts => opts.Ignore())
            .ForMember(dest => dest.Versions, opts => opts.Ignore())
            .ForMember(dest => dest.CreatedAt, opts => opts.Ignore())
            .ForMember(dest => dest.UpdatedAt, opts => opts.Ignore())
            .ForMember(dest => dest.GroupAccessRules, opts => opts.Ignore())
            .ForMember(dest => dest.PersonalAccessRules, opts => opts.Ignore());
    }
}