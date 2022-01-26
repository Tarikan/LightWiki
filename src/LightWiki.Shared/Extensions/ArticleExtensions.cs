using System;
using System.Linq;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;

namespace LightWiki.Shared.Extensions;

public static class ArticleExtensions
{
    public static ArticleAccessRule GetHighestPriorityRule(this Article article)
    {
        if (article.GroupAccessRules is null ||
            article.PersonalAccessRules is null)
        {
            throw new ArgumentException("Article's personal or group rules is null");
        }

        if (article.PersonalAccessRules.Count > 1)
        {
            throw new ArgumentException("Article's personal access rules is invalid");
        }

        if (article.PersonalAccessRules.Any())
        {
            return article.PersonalAccessRules.First().ArticleAccessRule;
        }

        if (article.GroupAccessRules.Any())
        {
            return article.GroupAccessRules
                .Select(gar => gar.ArticleAccessRule)
                .Aggregate(ArticleAccessRule.None, (acc, a) => acc | a);
        }

        return article.GlobalAccessRule;
    }
}