using System.Linq;
using FluentValidation;
using LightWiki.Data;
using LightWiki.Features.Articles.Requests;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Features.Articles.Validators;

public class GetArticleBySlugValidator : AbstractValidator<GetArticleBySlug>
{
    public GetArticleBySlugValidator(WikiContext wikiContext)
    {
        RuleFor(r => r.ArticleNameSlug)
            .Cascade(CascadeMode.Stop)
            .NotEmpty();

        RuleFor(r => r.WorkspaceNameSlug)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .CustomAsync(async (workspaceName, ctx, _) =>
            {
                var workspace = await wikiContext.Workspaces
                    .AnyAsync(a => a.Slug == workspaceName);
                if (!workspace)
                {
                    ctx.AddFailure("Workspace not found");
                }
            });

        RuleFor(r => r)
            .CustomAsync(async (request, ctx, _) =>
            {
                if (!(await wikiContext.Articles.AnyAsync(a => a.Slug == request.ArticleNameSlug &&
                                                               a.Workspace.Slug == request.WorkspaceNameSlug)))
                {
                    ctx.AddFailure("ArticleNameSlug", "Article not found");
                }
            });
    }
}