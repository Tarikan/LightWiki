using FluentValidation;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Validators;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Shared.Validation;

public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, int> UserShouldHaveAccessToArticle<T>(
        this IRuleBuilder<T, int> ruleBuilder,
        DbSet<Article> dataSet,
        IAuthorizedUserProvider authorizedUserProvider,
        ArticleAccessRule minimalRule)
    {
        return ruleBuilder.NotEmpty()
            .SetValidator(
                new ArticleAccessValidator(dataSet, authorizedUserProvider, minimalRule));
    }

    public static IRuleBuilderOptions<T, int> UserShouldHaveAccessToGroup<T>(
        this IRuleBuilder<T, int> ruleBuilder,
        DbSet<Group> dataSet,
        IAuthorizedUserProvider authorizedUserProvider,
        GroupAccessRule minimalRule)
    {
        return ruleBuilder.NotEmpty()
            .SetValidator(new GroupAccessValidator(dataSet, authorizedUserProvider, minimalRule));
    }

    public static IRuleBuilderOptions<T, int> UserShouldHaveAccessToWorkspace<T>(
        this IRuleBuilder<T, int> ruleBuilder,
        DbSet<Workspace> dataSet,
        IAuthorizedUserProvider authorizedUserProvider,
        WorkspaceAccessRule minimalRule)
    {
        return ruleBuilder.NotEmpty()
            .SetValidator(new WorkspaceAccessValidator(
                dataSet,
                authorizedUserProvider,
                minimalRule));
    }
}