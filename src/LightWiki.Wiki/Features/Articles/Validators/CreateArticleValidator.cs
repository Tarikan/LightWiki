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
            .CustomAsync(async (name, ctx, _) =>
            {
                if (await wikiContext.Articles.AnyAsync(a => a.Name == name))
                {
                    ctx.AddFailure("Article with such name already exists");
                }
            });

        RuleFor(r => r.WorkspaceId)
            .UserShouldHaveAccessToWorkspace(
                wikiContext.Workspaces,
                authorizedUserProvider,
                WorkspaceAccessRule.CreateArticle);

        RuleFor(r => r.ParentId)
            .CustomAsync(async (parentId, ctx, _) =>
            {
                if (parentId is null)
                {
                    return;
                }

                var article = await wikiContext.Articles.FindAsync(parentId.Value);

                if (article is null)
                {
                    ctx.AddFailure($"article with id {parentId} not found");
                }
            });
    }
}