using System.Linq;
using FluentValidation;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using LightWiki.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Shared.Validation;

public class WorkspaceAccessValidator : AbstractValidator<int>
{
    public WorkspaceAccessValidator(
        DbSet<Workspace> workspaces,
        IAuthorizedUserProvider authorizedUserProvider,
        WorkspaceAccessRule minimalRule,
        bool allowUnauthenticated)
    {
        RuleFor(r => r)
            .CustomAsync(async (id, ctx, _) =>
            {
                Workspace workspace;
                var userContext = await authorizedUserProvider.GetUser();

                if (userContext is null)
                {
                    if (!allowUnauthenticated)
                    {
                        ctx.AddFailure("Access denied");
                        return;
                    }

                    workspace = await workspaces.FindAsync(id);

                    if (!workspace.WorkspaceAccessRule.HasFlag(minimalRule))
                    {
                        ctx.AddFailure("Access denied");
                    }

                    return;
                }

                workspace = await workspaces
                    .Include(a => a.PersonalAccessRules
                        .Where(par => par.UserId == userContext.Id))
                    .Include(a => a.GroupAccessRules
                        .Where(gar => gar.Group.Users.Any(u => u.Id == userContext.Id)))
                    .SingleAsync(a => a.Id == id);

                var rule = GetHighestLevelRule(workspace);

                if (!rule.HasFlag(minimalRule))
                {
                    ctx.AddFailure("Access denied");
                }
            });
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