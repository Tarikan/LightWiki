using System.Linq;
using FluentValidation;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using LightWiki.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Shared.Validation;

public class GroupAccessValidator : AbstractValidator<int>
{
    public GroupAccessValidator(
        DbSet<Group> groups,
        IAuthorizedUserProvider authorizedUserProvider,
        GroupAccessRule requiredRule)
    {
        RuleFor(r => r)
            .CustomAsync(async (groupId, ctx, _) =>
            {
                var userContext = await authorizedUserProvider.GetUser();
                var group = await groups
                    .Include(g => g.GroupPersonalAccessRules
                        .Where(r => r.UserId == userContext.Id))
                    .SingleAsync(g => g.Id == groupId);

                if (group.GroupPersonalAccessRules.Any())
                {
                    var personalRule = group.GroupPersonalAccessRules.First().AccessRule;

                    if (!personalRule.HasFlag(requiredRule))
                    {
                        ctx.AddFailure("Access denied");
                        return;
                    }
                }

                if (!group.GroupAccessRule.HasFlag(requiredRule))
                {
                    ctx.AddFailure("Access denied");
                    return;
                }
            });
    }
}