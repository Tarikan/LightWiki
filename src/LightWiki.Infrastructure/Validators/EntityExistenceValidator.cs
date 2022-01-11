using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Infrastructure.Validators;

public class EntityExistenceValidator<TEntity, TProperty> : AbstractValidator<TProperty>
    where TEntity : class
{
    public EntityExistenceValidator(DbSet<TEntity> dataSet)
    {
        RuleFor(c => c)
            .CustomAsync(async (id, c, _) =>
            {
                var user = await dataSet.FindAsync(id);

                if (user == null)
                {
                    c.AddFailure($"{typeof(TEntity).Name} can not be found!");
                }
            });
    }
}