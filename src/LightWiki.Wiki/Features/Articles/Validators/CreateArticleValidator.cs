using System;
using System.Linq;
using FluentValidation;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Shared.Validation;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Features.Articles.Validators;

public class CreateArticleValidator : AbstractValidator<CreateArticle>
{
    public CreateArticleValidator(WikiContext wikiContext, IAuthorizedUserProvider authorizedUserProvider)
    {
        RuleFor(m => m.Name).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(64)
            .Must(n => n.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)));

        RuleFor(r => r.WorkspaceId)
            .UserShouldHaveAccessToWorkspace(
                wikiContext.Workspaces,
                authorizedUserProvider,
                WorkspaceAccessRule.CreateArticle);

        RuleFor(r => r)
            .CustomAsync(async (request, ctx, _) =>
            {
                if (await wikiContext.Articles.AnyAsync(
                        a => string.Equals(a.Name.ToUpper(), request.Name.ToUpper()) &&
                             a.WorkspaceId == request.WorkspaceId))
                {
                    ctx.AddFailure("Name", "Article with such name already exists");
                }
            });

        RuleFor(r => new { r.ParentId, r.WorkspaceId })
            .CustomAsync(async (tuple, ctx, _) =>
            {
                if (tuple.ParentId is null)
                {
                    return;
                }

                var article = await wikiContext.Articles.FindAsync(tuple.ParentId.Value);

                if (article is null || article.WorkspaceId != tuple.WorkspaceId)
                {
                    ctx.AddFailure($"Article with id {tuple.ParentId} not found");
                }
            });
    }
}