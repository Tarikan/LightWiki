using FluentValidation;
using LightWiki.Data;
using LightWiki.Data.Mongo.Repositories;
using LightWiki.Domain.Enums;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Configuration;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Validators;
using LightWiki.Shared.Validation;

namespace LightWiki.Features.Articles.Validators;

public class GetArticleValidator : AbstractValidator<GetArticle>
{
    public GetArticleValidator(
        WikiContext wikiContext,
        IAuthorizedUserProvider authorizedUserProvider,
        IArticleHierarchyNodeRepository articleHierarchyNodeRepository)
    {
        RuleFor(x => x.ArticleId)
            .Cascade(CascadeMode.Stop)
            .EntityShouldExist(wikiContext.Articles)
            .WithErrorCode(FailCode.BadRequest.ToString())
            .UserShouldHaveAccessToArticle(
                wikiContext.Articles,
                authorizedUserProvider,
                articleHierarchyNodeRepository,
                ArticleAccessRule.Read)
            .WithErrorCode(FailCode.Forbidden.ToString());
    }
}