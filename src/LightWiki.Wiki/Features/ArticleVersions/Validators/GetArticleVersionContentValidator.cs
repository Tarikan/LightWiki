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

public class GetArticleVersionContentValidator : AbstractValidator<GetArticleVersionContent>
{
    public GetArticleVersionContentValidator(
        WikiContext wikiContext,
        IAuthorizedUserProvider authorizedUserProvider,
        IArticleHierarchyNodeRepository articleHierarchyNodeRepository)
    {
        RuleFor(r => r.ArticleVersionId)
            .Cascade(CascadeMode.Stop)
            .EntityShouldExist(wikiContext.ArticleVersions)
            .WithErrorCode(FailCode.BadRequest.ToString())
            .MustAsync(async (request, id, _) =>
            {
                var articleVersion = await wikiContext.ArticleVersions
                    .FindAsync(request.ArticleVersionId);

                var accessValidator = new ArticleAccessValidator(
                    wikiContext.Articles,
                    authorizedUserProvider,
                    articleHierarchyNodeRepository,
                    ArticleAccessRule.Read);

                var validationResult = await accessValidator.ValidateAsync(articleVersion.ArticleId);

                if (!validationResult.IsValid)
                {
                    return false;
                }

                return true;
            })
            .WithErrorCode(FailCode.Forbidden.ToString());
    }
}