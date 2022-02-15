using FluentValidation;
using LightWiki.Data;
using LightWiki.Data.Mongo.Repositories;
using LightWiki.Domain.Enums;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Models;
using LightWiki.Infrastructure.Validators;
using LightWiki.Shared.Validation;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Features.Articles.Validators;

public class RemoveArticleAccessValidator : AbstractValidator<RemoveArticleAccess>
{
    public RemoveArticleAccessValidator(
        WikiContext wikiContext,
        IAuthorizedUserProvider authorizedUserProvider,
        IArticleHierarchyNodeRepository articleHierarchyNodeRepository)
    {
        RuleFor(r => r.ArticleId)
            .EntityShouldExist(wikiContext.Articles)
            .WithErrorCode(FailCode.BadRequest.ToString())
            .UserShouldHaveAccessToArticle(
                wikiContext.Articles,
                authorizedUserProvider,
                articleHierarchyNodeRepository,
                ArticleAccessRule.Modify)
            .WithErrorCode(FailCode.Forbidden.ToString());

        RuleFor(r => r.PartyId)
            .EntityShouldExist(wikiContext.Parties);

        RuleFor(r => r)
            .CustomAsync(async (r, ctx, _) =>
            {
                if (!await wikiContext.ArticleAccesses
                        .AnyAsync(u => u.PartyId == r.PartyId &&
                                       u.ArticleId == r.ArticleId))
                {
                    ctx.AddFailure("Rule not found");
                }
            });
    }
}