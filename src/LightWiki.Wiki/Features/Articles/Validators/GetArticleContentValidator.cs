using FluentValidation;
using LightWiki.Data;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Validators;

namespace LightWiki.Features.Articles.Validators;

public class GetArticleContentValidator : AbstractValidator<GetArticleContent>
{
    public GetArticleContentValidator(WikiContext wikiContext)
    {
        RuleFor(r => r.ArticleId).EntityShouldExist(wikiContext.Articles);
    }
}