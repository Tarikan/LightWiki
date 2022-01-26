using System;
using System.Collections.Generic;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using LightWiki.Features.Articles.Requests.Models;

namespace LightWiki.Features.Articles.Responses.Models;

public class ArticleModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Slug { get; set; }

    public int UserId { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public RequestAccessModel GlobalAccessRule { get; set; }

    public List<ArticleGroupAccessRuleModel> GroupAccessRules { get; set; }

    public List<ArticlePersonalAccessRuleModel> PersonalAccessRules { get; set; }
}