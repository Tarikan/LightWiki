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

public class RemoveArticleValidator : AbstractValidator<RemoveArticle>
{
    public RemoveArticleValidator(
        WikiContext wikiContext,
        IAuthorizedUserProvider authorizedUserProvider,
        IArticleHierarchyNodeRepository articleHierarchyNodeRepository)
    {
        RuleFor(r => r.ArticleId)
            .Cascade(CascadeMode.Stop)
            .EntityShouldExist(wikiContext.Articles)
            .UserShouldHaveAccessToArticle(
                wikiContext.Articles,
                authorizedUserProvider,
                articleHierarchyNodeRepository,
                ArticleAccessRule.Modify)
            .WithErrorCode(FailCode.Forbidden.ToString())
            .CustomAsync(async (id, ctx, _) =>
            {
                if (await wikiContext.Workspaces.AnyAsync(w => w.RootArticleId == id))
                {
                    ctx.AddFailure("Cannot remove root article");
                }
            });
    }
}