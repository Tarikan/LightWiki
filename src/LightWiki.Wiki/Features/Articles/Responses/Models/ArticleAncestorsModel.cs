using System.Collections.Generic;

namespace LightWiki.Features.Articles.Responses.Models;

public class ArticleAncestorsModel
{
    public int ArticleId { get; set; }

    public List<int> ArticleAncestors { get; set; }
}