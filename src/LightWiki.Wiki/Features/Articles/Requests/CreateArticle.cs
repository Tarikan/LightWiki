using LightWiki.Domain.Enums;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Articles.Requests
{
    public class CreateArticle : IRequest<OneOf<SuccessWithId<int>, Fail>>
    {
        public string Name { get; set; }

        public ArticleVisibility ArticleVisibility { get; set; }

        public ArticleModificationAccess ArticleModificationAccess { get; set; }
    }
}