using FluentValidation;
using LightWiki.Data;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Validators;

namespace LightWiki.Features.Articles.Validators;

public class UpdateArticleValidator : AbstractValidator<UpdateArticle>
{
    public UpdateArticleValidator(WikiContext context)
    {
        RuleFor(m => m.Id).EntityShouldExist(context.Articles);
    }
}