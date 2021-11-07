using FluentValidation;
using LightWiki.Data;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Validators;

namespace LightWiki.Features.Articles.Validators
{
    public class GetArticleValidator : AbstractValidator<GetArticle>
    {
        public GetArticleValidator(WikiContext wikiContext)
        {
            RuleFor(r => r.ArticleId).EntityShouldExist(wikiContext.Articles);
        }
    }
}