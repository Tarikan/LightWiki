using FluentValidation;
using LightWiki.Data;
using LightWiki.Data.Mongo.Repositories;
using LightWiki.Domain.Enums;
using LightWiki.Features.ArticleVersions.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Validators;
using LightWiki.Shared.Validation;

namespace LightWiki.Features.ArticleVersions.Validators;

public class GetArticleVersionsValidator : AbstractValidator<GetArticleVersions>
{
    public GetArticleVersionsValidator(
        WikiContext context,
        IAuthorizedUserProvider authorizedUserProvider,
        IArticleHierarchyNodeRepository articleHierarchyNodeRepository)
    {
        RuleFor(r => r.ArticleId)
            .Cascade(CascadeMode.Stop)
            .EntityShouldExist(context.Articles)
            .WithErrorCode(FailCode.BadRequest.ToString())
            .UserShouldHaveAccessToArticle(
                context.Articles,
                authorizedUserProvider,
                articleHierarchyNodeRepository,
                ArticleAccessRule.Read)
            .WithErrorCode(FailCode.Forbidden.ToString());
    }
}