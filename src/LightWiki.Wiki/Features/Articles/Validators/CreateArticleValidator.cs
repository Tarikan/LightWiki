using FluentValidation;
using LightWiki.Features.Articles.Requests;

namespace LightWiki.Features.Articles.Validators;

public class CreateArticleValidator : AbstractValidator<CreateArticle>
{
    public CreateArticleValidator()
    {
        RuleFor(m => m.Name).NotEmpty().MaximumLength(64);
    }
}