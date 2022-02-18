using System;
using System.Linq;
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

public class UpdateArticleValidator : AbstractValidator<UpdateArticle>
{
    public UpdateArticleValidator(
        WikiContext wikiContext,
        IAuthorizedUserProvider authorizedUserProvider,
        IArticleHierarchyNodeRepository articleHierarchyNodeRepository)
    {
        RuleFor(r => r.Id).Cascade(CascadeMode.Stop)
            .EntityShouldExist(wikiContext.Articles)
            .WithErrorCode(FailCode.BadRequest.ToString())
            .UserShouldHaveAccessToArticle(
                wikiContext.Articles,
                authorizedUserProvider,
                articleHierarchyNodeRepository,
                ArticleAccessRule.Modify)
            .WithErrorCode(FailCode.Forbidden.ToString());

        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(64)
            .Must(n => n.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)));

        RuleFor(r => new { r.Name, r.Id })
            .CustomAsync(async (tuple, ctx, _) =>
            {
                var article = await wikiContext.Articles.FindAsync(tuple.Id);

                if (article is null)
                {
                    return;
                }

                if (await wikiContext.Articles.AnyAsync(a =>
                        string.Equals(a.Name.ToUpper(), tuple.Name.ToUpper()) &&
                        a.WorkspaceId == article.WorkspaceId))
                {
                    ctx.AddFailure("Name", "Article with such name already exists");
                }
            });
    }
}