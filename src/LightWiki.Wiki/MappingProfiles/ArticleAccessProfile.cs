using AutoMapper;
using LightWiki.Domain.Models;
using LightWiki.Features.Articles.Responses.Models;

namespace LightWiki.MappingProfiles;

public class ArticleAccessProfile : Profile
{
    public ArticleAccessProfile()
    {
        CreateMap<ArticlePersonalAccessRule, ArticlePersonalAccessRuleModel>()
            .ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.UserId));

        CreateMap<ArticleGroupAccessRule, ArticleGroupAccessRuleModel>();
    }
}