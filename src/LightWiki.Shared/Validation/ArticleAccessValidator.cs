using System;
using System.Linq;
using FluentValidation;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using LightWiki.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Shared.Validation;

public class ArticleAccessValidator : AbstractValidator<int>
{
    public ArticleAccessValidator(
        DbSet<Article> articles,
        IAuthorizedUserProvider authorizedUserProvider,
        ArticleAccessRule minimalRule)
    {
        RuleFor(c => c)
            .CustomAsync(async (id, ctx, _) =>
            {
                Article article;
                var userContext = await authorizedUserProvider.GetUser();

                if (userContext is null)
                {
                    article = await articles
                        .Include(a => a.Workspace)
                        .SingleAsync(a => a.Id == id);

                    if (!article.Workspace.WorkspaceAccessRule.HasFlag(WorkspaceAccessRule.Browse) ||
                        !article.GlobalAccessRule.HasFlag(minimalRule))
                    {
                        ctx.AddFailure("Access denied");
                    }

                    return;
                }

                article = await articles
                    .Include(a => a.Workspace)
                    .ThenInclude(w => w.PersonalAccessRules
                        .Where(par => par.UserId == userContext.Id))
                    .Include(a => a.Workspace)
                    .ThenInclude(w => w.GroupAccessRules
                        .Where(gar => gar.Group.Users.Any(u => u.Id == userContext.Id)))
                    .Include(a => a.PersonalAccessRules
                        .Where(par => par.UserId == userContext.Id))
                    .Include(a => a.GroupAccessRules
                        .Where(gar => gar.Group.Users.Any(u => u.Id == userContext.Id)))
                    .SingleAsync(a => a.Id == id);

                if (!CheckWorkspaceAccess(article.Workspace, WorkspaceAccessRule.Browse) ||
                    !CheckArticleAccess(article, minimalRule))
                {
                    ctx.AddFailure("Access denied");
                }
            });
    }

    private static bool CheckArticleAccess(Article article, ArticleAccessRule minimalRule)
    {
        var workspaceRule = GetHighestLevelRule(article.Workspace);

        var converted = RuleConverter.Convert(workspaceRule);

        var rule = GetHighestLevelRule(article) | converted;

        if (!rule.HasFlag(minimalRule))
        {
            return false;
        }

        return true;
    }

    private static bool CheckWorkspaceAccess(Workspace workspace, WorkspaceAccessRule minimalRule)
    {
        var rule = GetHighestLevelRule(workspace);

        if (!rule.HasFlag(minimalRule))
        {
            return false;
        }

        return true;
    }

    private static ArticleAccessRule GetHighestLevelRule(Article article)
    {
        if (article.PersonalAccessRules.Any())
        {
            return article.PersonalAccessRules.First().ArticleAccessRule;
        }

        if (article.GroupAccessRules.Any())
        {
            article.GroupAccessRules
                .Select(gar => gar.ArticleAccessRule)
                .Aggregate(ArticleAccessRule.None, (acc, x) => acc | x);
        }

        return article.GlobalAccessRule;
    }

    private static WorkspaceAccessRule GetHighestLevelRule(Workspace workspace)
    {
        if (workspace.PersonalAccessRules.Any())
        {
            return workspace.PersonalAccessRules.First().WorkspaceAccessRule;
        }

        if (workspace.GroupAccessRules.Any())
        {
            workspace.GroupAccessRules
                .Select(gar => gar.WorkspaceAccessRule)
                .Aggregate(WorkspaceAccessRule.None, (acc, x) => acc | x);
        }

        return workspace.WorkspaceAccessRule;
    }
}