using FluentValidation;
using LightWiki.Data;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Validators;

namespace LightWiki.Features.Articles.Validators;

public class UpdateArticleContentValidator : AbstractValidator<UpdateArticleContent>
{
    public UpdateArticleContentValidator(WikiContext wikiContext)
    {
        RuleFor(x => x.ArticleId).EntityShouldExist(wikiContext.Articles);

        RuleFor(x => x.Text).NotEmpty();
    }
}