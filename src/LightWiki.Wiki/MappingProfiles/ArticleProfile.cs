using AutoMapper;
using LightWiki.Domain.Models;
using LightWiki.Features.Articles.Requests;
using LightWiki.Features.Articles.Responses.Models;

namespace LightWiki.MappingProfiles
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<Article, ArticleWithContentModel>()
                .ForMember(m => m.Content, opts => opts.Ignore());
            CreateMap<CreateArticle, Article>();
        }
    }
}