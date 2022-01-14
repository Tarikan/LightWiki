using System.Linq;
using FluentValidation;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Configuration;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Validators;
using LightWiki.Shared.Validation;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Features.Articles.Validators;

public class GetArticleValidator : AbstractValidator<GetArticle>
{
    public GetArticleValidator(
        WikiContext wikiContext,
        IAuthorizedUserProvider authorizedUserProvider,
        AppConfiguration settings)
    {
        RuleFor(x => x.ArticleId)
            .Cascade(CascadeMode.Stop)
            .EntityShouldExist(wikiContext.Articles)
            .WithErrorCode(FailCode.BadRequest.ToString())
            .UserShouldHaveAccessToArticle(
                wikiContext.Articles,
                authorizedUserProvider,
                ArticleAccessRule.Read,
                settings.AllowUnauthorizedUse)
            .WithErrorCode(FailCode.Forbidden.ToString());
    }
}