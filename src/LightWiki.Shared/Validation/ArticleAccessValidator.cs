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
        ArticleAccessRule minimalRule,
        bool allowUnauthenticated)
    {
        RuleFor(c => c)
            .CustomAsync(async (id, ctx, _) =>
            {
                Article article;
                var userContext = await authorizedUserProvider.GetUser();

                if (userContext is null)
                {
                    if (!allowUnauthenticated)
                    {
                        ctx.AddFailure("Access denied");
                        return;
                    }

                    article = await articles.FindAsync(id);

                    if (article.GlobalAccessRule < minimalRule)
                    {
                        ctx.AddFailure("Access denied");
                    }

                    return;
                }

                article = await articles
                    .Include(a => a.PersonalAccessRules
                        .Where(par => par.UserId == userContext.Id))
                    .Include(a => a.GroupAccessRules
                        .Where(gar => gar.Group.Users.Any(u => u.Id == userContext.Id)))
                    .SingleAsync(a => a.Id == id);

                if (article.PersonalAccessRules.Any())
                {
                    var personalRule = article.PersonalAccessRules.First();
                    if (!personalRule.ArticleAccessRule.HasFlag(minimalRule))
                    {
                        ctx.AddFailure("Access denied");
                    }

                    return;
                }

                if (article.GroupAccessRules.Any())
                {
                    var groupRule = article.GroupAccessRules.First();

                    if (!groupRule.ArticleAccessRule.HasFlag(minimalRule))
                    {
                        ctx.AddFailure("Access denied");
                    }

                    return;
                }

                if (!article.GlobalAccessRule.HasFlag(minimalRule))
                {
                    ctx.AddFailure("Access denied");
                }
            });
    }
}