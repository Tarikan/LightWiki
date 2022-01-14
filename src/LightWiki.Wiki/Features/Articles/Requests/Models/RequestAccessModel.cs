using System;
using System.Collections.Generic;
using System.Linq;
using LightWiki.Domain.Enums;
using Newtonsoft.Json;

namespace LightWiki.Features.Articles.Requests.Models;

public class RequestAccessModel
{
    public RequestAccessModel(ArticleAccessRule rules)
    {
        Rules = new List<ArticleAccessRule>();
        foreach (var rule in Enum.GetValues<ArticleAccessRule>())
        {
            if (rule != ArticleAccessRule.All & (rules & rule) != 0)
            {
                Rules.Add(rule);
            }
        }
    }

    public List<ArticleAccessRule> Rules { get; set; }

    [JsonIgnore]
    public ArticleAccessRule SingleRule =>
        Rules.Aggregate(ArticleAccessRule.None, (acc, x) => acc | x);
}