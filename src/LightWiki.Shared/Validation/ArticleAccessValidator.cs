using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using LightWiki.Data.Mongo.Repositories;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Extensions;
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
                        .IncludeDefaultAccessRules()
                        .Where(a => idsToSelect.Contains(a.Id))
                        .ToListAsync();
                    rule = articles.Select(a => a.ArticleAccesses.GetHighestPriorityRule())
                        .Aggregate(ArticleAccessRule.None, (acc, a) => a | acc);
                }
                else
                {
                    articles = await articleSet
                        .IncludeAccessRules(userContext.Id)
                        .Where(a => idsToSelect.Contains(a.Id))
                        .ToListAsync();

                    rule = articles.Select(a => a.ArticleAccesses.GetHighestPriorityRule())
                        .Aggregate(ArticleAccessRule.None, (acc, a) => a | acc);
                }

                if (!rule.HasFlag(ruleForAncestors) ||
                    !articles.Single(a => a.Id == id).ArticleAccesses.GetHighestPriorityRule().HasFlag(minimalRule))
                {
                    ctx.AddFailure("Access denied");
                }
            });
    }
}