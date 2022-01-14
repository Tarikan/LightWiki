using FluentValidation;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Validators;
using LightWiki.Shared.Validation;

namespace LightWiki.Features.Articles.Validators;

public class UpdateArticleContentValidator : AbstractValidator<UpdateArticleContent>
{
    public UpdateArticleContentValidator(
        WikiContext wikiContext,
        IAuthorizedUserProvider authorizedUserProvider)
    {
        RuleFor(x => x.ArticleId)
            .Cascade(CascadeMode.Stop)
            .EntityShouldExist(wikiContext.Articles)
            .WithErrorCode(FailCode.BadRequest.ToString())
            .UserShouldHaveAccess(wikiContext.Articles, authorizedUserProvider, ArticleAccessRule.Write)
            .WithErrorCode(FailCode.Forbidden.ToString());

        RuleFor(x => x.Text).NotEmpty();
    }
}