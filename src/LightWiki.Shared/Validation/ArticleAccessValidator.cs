using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using LightWiki.Data.Mongo.Repositories;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using LightWiki.Infrastructure.Auth;
using LightWiki.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Shared.Validation;

public class ArticleAccessValidator : AbstractValidator<int>
{
    public ArticleAccessValidator(
        DbSet<Article> articleSet,
        IAuthorizedUserProvider authorizedUserProvider,
        IArticleHierarchyNodeRepository articleHierarchyNodeRepository,
        ArticleAccessRule minimalRule,
        ArticleAccessRule ruleForAncestors = ArticleAccessRule.Read)
    {
        RuleFor(c => c)
            .CustomAsync(async (id, ctx, _) =>
            {
                var userContext = await authorizedUserProvider.GetUserOrDefault();
                var idsToSelect = await articleHierarchyNodeRepository.GetAncestors(id);
                idsToSelect.Add(id);
                List<Article> articles;
                ArticleAccessRule rule;

                if (userContext is null)
                {
                    articles = await articleSet
                        .Where(a => idsToSelect.Contains(a.Id))
                        .ToListAsync();
                    rule = articles.Select(a => a.GlobalAccessRule)
                        .Aggregate(ArticleAccessRule.All, (acc, a) => a & acc);
                }
                else
                {
                    articles = await articleSet
                        .Include(a => a.PersonalAccessRules
                            .Where(par => par.UserId == userContext.Id))
                        .Include(a => a.GroupAccessRules
                            .Where(gar => gar.Group.Users.Any(u => u.Id == userContext.Id)))
                        .Where(a => idsToSelect.Contains(a.Id))
                        .ToListAsync();

                    rule = articles.Select(a => a.GetHighestPriorityRule())
                        .Aggregate(ArticleAccessRule.All, (acc, a) => a & acc);
                }

                if (!rule.HasFlag(ruleForAncestors) ||
                    !articles.Single(a => a.Id == id).GetHighestPriorityRule().HasFlag(minimalRule))
                {
                    ctx.AddFailure("Access denied");
                }
            });
    }
}