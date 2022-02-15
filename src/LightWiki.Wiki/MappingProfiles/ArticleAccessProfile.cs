using AutoMapper;
using LightWiki.Domain.Models;
using LightWiki.Features.Articles.Responses.Models;

namespace LightWiki.MappingProfiles;

public class ArticleAccessProfile : Profile
{
    public ArticleAccessProfile()
    {
        CreateMap<ArticleAccess, ArticleAccessModel>();
    }
}