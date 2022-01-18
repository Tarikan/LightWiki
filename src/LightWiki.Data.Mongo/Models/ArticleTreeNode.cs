using System.Collections.Generic;

namespace LightWiki.Data.Mongo.Models;

public class ArticleTreeNode
{
    public int ArticleId { get; set; }

    public string Name { get; set; }

    public List<ArticleTreeNode> Children { get; set; }
}