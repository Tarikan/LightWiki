using System;
using System.Collections.Generic;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using LightWiki.Features.Articles.Requests.Models;
using LightWiki.Features.ArticleVersions.Responses.Models;
using LightWiki.Features.Users.Responses.Models;

namespace LightWiki.Features.Articles.Responses.Models;

public class ArticleModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Slug { get; set; }

    public int UserId { get; set; }

    public UserModel User { get; set; }

    public ArticleVersionModel LastArticleVersion { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<ArticleAccessModel> ArticleAccesses { get; set; }

    public ArticleAccessRule ArticleAccessRuleForCaller { get; set; }
}