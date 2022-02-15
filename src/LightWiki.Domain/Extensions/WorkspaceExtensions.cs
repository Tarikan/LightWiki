using System;
using System.Collections.Generic;
using System.Linq;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;

namespace LightWiki.Domain.Extensions
{
    public static class WorkspaceExtensions
    {
        /// <summary>
        /// Should be used in Include method when retrieving related workspaces from data source
        /// </summary>
        /// <param name="workspaceAccesses">Entity for workspace access</param>
        /// <param name="userId">Id of user</param>
        /// <returns>Filtered collection</returns>
        public static IEnumerable<WorkspaceAccess> WhereUserIsRelated(
            this IEnumerable<WorkspaceAccess> workspaceAccesses,
            int userId)
        {
            return workspaceAccesses
                .Where(a => a.Party.Id == userId ||
                            a.Party.PartyType == PartyType.Group &&
                            a.Party.Groups.Single().Users.Any(u => u.Id == userId));
        }

        /// <summary>
        /// Should be used in "Where" method to filter workspaces
        /// </summary>
        /// <param name="workspace">Workspace</param>
        /// <param name="userId">Id of user</param>
        /// <returns>Result of comparison</returns>
        public static bool WhereUserIsRelated(
            this Workspace workspace,
            int userId)
        {
            return workspace.WorkspaceAccesses.Any(
                       a => a.Party.PartyType == PartyType.User &&
                            a.WorkspaceAccessRule.HasFlag(WorkspaceAccessRule.Browse)) ||
                   (workspace.WorkspaceAccesses.All(a => a.Party.PartyType != PartyType.User) &&
                    workspace.WorkspaceAccesses.Any(a => a.WorkspaceAccessRule.HasFlag(WorkspaceAccessRule.Browse)));
        }

        /// <summary>
        /// Return suitable rule
        /// </summary>
        /// <param name="workspaceAccesses">Workspace</param>
        /// <returns>Rule</returns>
        public static WorkspaceAccessRule GetHighestLevelRule(this IEnumerable<WorkspaceAccess> workspaceAccesses)
        {
            if (workspaceAccesses is null)
            {
                throw new ArgumentException("WorkspaceAccesses is null");
            }

            if (workspaceAccesses.Any(a => a.Party.PartyType == PartyType.User))
            {
                return workspaceAccesses.Single(a => a.Party.PartyType == PartyType.User).WorkspaceAccessRule;
            }

            return workspaceAccesses.Aggregate(
                WorkspaceAccessRule.None,
                (acc, a) => acc | a.WorkspaceAccessRule);
        }
    }
}