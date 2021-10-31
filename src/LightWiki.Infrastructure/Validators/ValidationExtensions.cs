using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Infrastructure.Validators
{
    public static class ValidationExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> EntityShouldExist<T, TProperty, TEntity>(this IRuleBuilder<T, TProperty> ruleBuilder, DbSet<TEntity> dataSet)
            where TEntity : class
        {
            return ruleBuilder.NotEmpty().SetValidator(new EntityExistenceValidator<TEntity, TProperty>(dataSet));
        }
    }
}