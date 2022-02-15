using System.Linq;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Shared.Extensions;

public static class WorkspaceQueryExtensions
{
    public static IQueryable<Workspace> IncludeAccessRules(this IQueryable<Workspace> workspaces, int userId)
    {
        return workspaces.Include(w => w.WorkspaceAccesses.Where(a => a.Party.PartyType == PartyType.User &&
                                                                      a.Party.Users.First().Id == userId ||
                                                                      a.Party.PartyType == PartyType.Group &&
                                                                      a.Party.Groups.Single().Users
                                                                          .Any(u => u.Id == userId)))
            .ThenInclude(r => r.Party);
    }

    public static IQueryable<Workspace> IncludeDefaultAccessRules(this IQueryable<Workspace> workspaces)
    {
        return workspaces.Include(w => w.WorkspaceAccesses
                .Where(a => a.Party.PartyType == PartyType.Group &&
                            a.Party.Groups.First().GroupType == GroupType.Default))
            .ThenInclude(r => r.Party);
    }

    public static IQueryable<Workspace> WhereUserHasAccess(this IQueryable<Workspace> workspaces, int userId, WorkspaceAccessRule workspaceAccessRule)
    {
        return workspaces.Where(w => w.WorkspaceAccesses.Any(
                                         a => a.Party.PartyType == PartyType.User &&
                                              a.Party.Users.First().Id == userId &&
                                              a.WorkspaceAccessRule.HasFlag(workspaceAccessRule)) ||
                                     (w.WorkspaceAccesses.All(a => a.Party.PartyType != PartyType.User) &&
                                      w.WorkspaceAccesses.Any(a =>
                                          a.Party.PartyType == PartyType.Group &&
                                          a.Party.Groups.First().Users.Any(u => u.Id == userId) &&
                                          a.WorkspaceAccessRule.HasFlag(workspaceAccessRule))));
    }
}