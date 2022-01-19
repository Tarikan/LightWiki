using System.Linq;
using FluentValidation;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Features.Articles.Requests;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Validators;
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

        RuleFor(r => r)
            .CustomAsync(async (request, ctx, _) =>
            {
                var res = await wikiContext.Articles
                    .Where(a => request.ParentIds.Contains(a.Id) &&
                                a.WorkspaceId == request.WorkspaceId)
                    .CountAsync();

                if (request.ParentIds.Count != res)
                {
                    ctx.AddFailure("Some articles from parents not found");
                }
            });
    }
}