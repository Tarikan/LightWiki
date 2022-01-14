using FluentValidation;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using LightWiki.Infrastructure.Auth;
using LightWiki.Infrastructure.Validators;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Shared.Validation;

public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, int> UserShouldHaveAccess<T>(
        this IRuleBuilder<T, int> ruleBuilder,
        DbSet<Article> dataSet,
        IAuthorizedUserProvider authorizedUserProvider,
        ArticleAccessRule minimalRule,
        bool allowUnauthenticated = false)
    {
        return ruleBuilder.NotEmpty().SetValidator(new ArticleAccessValidator(dataSet, authorizedUserProvider, minimalRule, allowUnauthenticated));
    }
}