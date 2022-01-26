using System.Collections.Generic;

namespace LightWiki.Data.Mongo.Models;

public class ArticleHierarchyNode : BaseModel
{
    public int ArticleId { get; set; }

    public int WorkspaceId { get; set; }

    public int? ParentId { get; set; }

    public List<int> AncestorIds { get; set; }
}