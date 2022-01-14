using FluentValidation;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Validators;
using LightWiki.Shared.Validation;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Features.Articles.Validators;

public class UpdateArticleValidator : AbstractValidator<UpdateArticle>
{
    public UpdateArticleValidator(WikiContext wikiContext, IAuthorizedUserProvider authorizedUserProvider)
    {
        RuleFor(r => r.Id).Cascade(CascadeMode.Stop)
            .EntityShouldExist(wikiContext.Articles)
            .WithErrorCode(FailCode.BadRequest.ToString())
            .UserShouldHaveAccess(wikiContext.Articles, authorizedUserProvider, ArticleAccessRule.Modify)
            .WithErrorCode(FailCode.Forbidden.ToString());

        RuleFor(r => r.Name)
            .CustomAsync(async (name, ctx, _) =>
            {
                if (await wikiContext.Articles.AnyAsync(a => a.Name == name, _))
                {
                    ctx.AddFailure("Article with such name already exists");
                }
            });
    }
}