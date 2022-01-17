using System;

namespace LightWiki.Features.ArticleVersions.Responses.Models;

public class ArticleVersionModel
{
    public int Id { get; set; }

    public int ArticleId { get; set; }

    public int UserId { get; set; }

    public DateTime CreationDate { get; set; }
}