using System;
using System.Linq;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;

namespace LightWiki.Shared.Extensions;

public static class WorkspaceExtensions
{
    public static WorkspaceAccessRule GetHighestLevelRule(this Workspace workspace)
    {
        if (workspace.PersonalAccessRules is null ||
            workspace.GroupAccessRules is null)
        {
            throw new ArgumentException();
        }

        if (workspace.PersonalAccessRules.Any())
        {
            return workspace.PersonalAccessRules.First().WorkspaceAccessRule;
        }

        if (workspace.GroupAccessRules.Any())
        {
            return workspace.GroupAccessRules.Select(gar => gar.WorkspaceAccessRule)
                .Aggregate(WorkspaceAccessRule.None, (acc, a) => acc | a);
        }

        return workspace.WorkspaceAccessRule;
    }
}