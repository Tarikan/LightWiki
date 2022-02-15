using FluentValidation;
using LightWiki.Data;
using LightWiki.Data.Mongo.Repositories;
using LightWiki.Domain.Enums;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Validators;
using LightWiki.Shared.Validation;

namespace LightWiki.Features.Articles.Validators;

public class AddArticleAccessValidator : AbstractValidator<AddArticleAccess>
{
    public AddArticleAccessValidator(
        WikiContext wikiContext,
        IAuthorizedUserProvider authorizedUserProvider,
        IArticleHierarchyNodeRepository articleHierarchyNodeRepository)
    {
        RuleFor(r => r.ArticleId)
            .Cascade(CascadeMode.Stop)
            .EntityShouldExist(wikiContext.Articles)
            .WithErrorCode(FailCode.BadRequest.ToString())
            .UserShouldHaveAccessToArticle(
                wikiContext.Articles,
                authorizedUserProvider,
                articleHierarchyNodeRepository,
                ArticleAccessRule.Modify)
            .WithErrorCode(FailCode.Forbidden.ToString());

        RuleFor(r => r.PartyId).EntityShouldExist(wikiContext.Parties);
    }
}