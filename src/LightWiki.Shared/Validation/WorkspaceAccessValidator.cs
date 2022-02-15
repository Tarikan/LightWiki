using System.Linq;
using FluentValidation;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Extensions;
using LightWiki.Domain.Models;
using LightWiki.Infrastructure.Auth;
using LightWiki.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Shared.Validation;

public class WorkspaceAccessValidator : AbstractValidator<int>
{
    public WorkspaceAccessValidator(
        DbSet<Workspace> workspaces,
        IAuthorizedUserProvider authorizedUserProvider,
        WorkspaceAccessRule minimalRule)
    {
        RuleFor(r => r)
            .CustomAsync(async (id, ctx, _) =>
            {
                Workspace workspace;
                var userContext = await authorizedUserProvider.GetUser();

                if (userContext is null)
                {
                    workspace = await workspaces
                        .IncludeDefaultAccessRules()
                        .SingleAsync(w => w.Id == id);

                    if (!workspace.WorkspaceAccesses.GetHighestLevelRule().HasFlag(minimalRule))
                    {
                        ctx.AddFailure("Access denied");
                    }

                    return;
                }

                workspace = await workspaces
                    .IncludeAccessRules(userContext.Id)
                    .WhereUserHasAccess(userContext.Id, WorkspaceAccessRule.Browse)
                    .SingleAsync(a => a.Id == id);

                var rule = workspace.WorkspaceAccesses.GetHighestLevelRule();

                if (!rule.HasFlag(minimalRule))
                {
                    ctx.AddFailure("Access denied");
                }
            });
    }
}