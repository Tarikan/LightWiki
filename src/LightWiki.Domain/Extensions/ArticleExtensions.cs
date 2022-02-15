using System;
using System.Collections.Generic;
using System.Linq;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;

namespace LightWiki.Domain.Extensions
{
    public static class ArticleExtensions
    {
        /// <summary>
        /// Should be used in Include method when retrieving related articles from data source
        /// </summary>
        /// <param name="articleAccesses">List of access entities</param>
        /// <param name="userId">Id of user</param>
        /// <returns>Filtered collection</returns>
        public static IEnumerable<ArticleAccess> WhereUserIsRelated(
            this IEnumerable<ArticleAccess> articleAccesses,
            int userId)
        {
            return articleAccesses
                .Where(a => a.Party.Id == userId ||
                            a.Party.PartyType == PartyType.Group &&
                            a.Party.Groups.Single().Users.Any(u => u.Id == userId));
        }

        /// <summary>
        /// Should be used in "Where" method to filter articles
        /// </summary>
        /// <param name="article">Article</param>
        /// <param name="userId">Id of user</param>
        /// <param name="articleAccessRule">Rule to check</param>
        /// <returns>Result of comparison</returns>
        public static bool WhereUserIsRelated(
            this Article article,
            int userId,
            ArticleAccessRule articleAccessRule = ArticleAccessRule.Read)
        {
            return article.ArticleAccesses.Any(a => a.Party.PartyType == PartyType.User &&
                                                    a.ArticleAccessRule.HasFlag(articleAccessRule)) ||
                   (article.ArticleAccesses.All(a => a.Party.PartyType != PartyType.User) &&
                    article.ArticleAccesses.Any(a => a.ArticleAccessRule.HasFlag(articleAccessRule)));
        }

        public static ArticleAccessRule GetHighestPriorityRule(this IEnumerable<ArticleAccess> accesses)
        {
            if (accesses is null)
            {
                throw new ArgumentException("ArticleAccesses is null");
            }

            if (accesses.Any(a => a.Party.PartyType == PartyType.User))
            {
                return accesses.Single(a => a.Party.PartyType == PartyType.User).ArticleAccessRule;
            }

            return accesses.Select(a => a.ArticleAccessRule).Aggregate(ArticleAccessRule.None, (acc, a) => acc | a);
        }
    }
}