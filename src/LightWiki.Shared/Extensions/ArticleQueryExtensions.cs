using System.Linq;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Shared.Extensions;

public static class ArticleQueryExtensions
{
    public static IQueryable<Article> IncludeAccessRules(this IQueryable<Article> articles, int userId)
    {
        return articles.Include(w => w.ArticleAccesses.Where(a => a.Party.PartyType == PartyType.User &&
                                                                  a.Party.Users.First().Id == userId ||
                                                                  a.Party.PartyType == PartyType.Group &&
                                                                  a.Party.Groups.Single().Users
                                                                      .Any(u => u.Id == userId)))
            .ThenInclude(r => r.Party);
    }

    public static IQueryable<Article> IncludeDefaultAccessRules(this IQueryable<Article> articles)
    {
        return articles.Include(w => w.ArticleAccesses
                .Where(a => a.Party.PartyType == PartyType.Group &&
                            a.Party.Groups.First().GroupType == GroupType.Default))
            .ThenInclude(r => r.Party);
    }

    public static IQueryable<Article> WhereUserHasAccess(
        this IQueryable<Article> articles,
        int userId,
        ArticleAccessRule articleAccessRule)
    {
        return articles.Where(a => a.ArticleAccesses.Any(
                                       a => a.Party.PartyType == PartyType.User &&
                                            a.Party.Users.First().Id == userId &&
                                            a.ArticleAccessRule.HasFlag(articleAccessRule)) ||
                                   (a.ArticleAccesses.All(a => a.Party.PartyType != PartyType.User) &&
                                    a.ArticleAccesses.Any(a =>
                                        a.Party.PartyType == PartyType.Group &&
                                        a.Party.Groups.First().Users.Any(u => u.Id == userId) &&
                                        a.ArticleAccessRule.HasFlag(articleAccessRule))));
    }

    public static IQueryable<Article> WhereDefaultUserHasAccess(
        this IQueryable<Article> articles,
        ArticleAccessRule articleAccessRule)
    {
        return articles.Where(a => a.ArticleAccesses.Any(acc => acc.Party.PartyType == PartyType.Group &&
                                                                acc.Party.Groups.Single().GroupType ==
                                                                GroupType.Default &&
                                                                acc.ArticleAccessRule.HasFlag(articleAccessRule)));
    }
}