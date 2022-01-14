using FluentValidation;
using LightWiki.Data;
using LightWiki.Features.Articles.Requests;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Features.Articles.Validators;

public class CreateArticleValidator : AbstractValidator<CreateArticle>
{
    public CreateArticleValidator(WikiContext wikiContext)
    {
        RuleFor(m => m.Name).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(64)
            .CustomAsync(async (name, ctx, _) =>
            {
                if (await wikiContext.Articles.AnyAsync(a => a.Name == name))
                {
                    ctx.AddFailure("Article with such name already exists");
                }
            });
    }
}