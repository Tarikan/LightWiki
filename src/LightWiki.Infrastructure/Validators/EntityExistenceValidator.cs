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
                var entity = await dataSet.FindAsync(id);

                if (entity == null)
                {
                    c.AddFailure($"{typeof(TEntity).Name} can not be found!");
                }
            });
    }
}