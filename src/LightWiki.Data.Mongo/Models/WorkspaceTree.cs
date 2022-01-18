using System.Collections.Generic;

namespace LightWiki.Data.Mongo.Models;

public class WorkspaceTree : BaseModel
{
    public string WorkspaceName { get; set; }

    public int WorkspaceId { get; set; }

    public List<ArticleTreeNode> ArticleTree { get; set; }
}