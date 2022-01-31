using FluentValidation;
using LightWiki.Data;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Validators;

namespace LightWiki.Features.Articles.Validators;

public class GetArticleAncestorsValidator : AbstractValidator<GetArticleAncestors>
{
    public GetArticleAncestorsValidator(WikiContext wikiContext)
    {
        RuleFor(x => x.ArticleId)
            .Cascade(CascadeMode.Stop)
            .EntityShouldExist(wikiContext.Articles);
    }
}