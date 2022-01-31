using System.Collections.Generic;
using System.Threading.Tasks;
using LightWiki.Data.Mongo.Models;

namespace LightWiki.Data.Mongo.Repositories;

public interface IArticleHierarchyNodeRepository : IBaseRepository<ArticleHierarchyNode>
{
    Task<List<int>> GetAncestors(int articleId);

    Task<List<ArticleHierarchyNode>> GetAncestorModels(int articleId);

    Task<List<ArticleHierarchyNode>> GetAllChildren(int articleId);

    Task<ArticleHierarchyNode> FindByArticleId(int articleId);

    Task<List<ArticleHierarchyNode>> GetTree(int articleId);
}