using System.Threading.Tasks;
using LightWiki.Data.Mongo.Models;

namespace LightWiki.Data.Mongo.Repositories
{
    public interface IArticleHtmlRepository : IBaseRepository<ArticleHtml>
    {
        Task<ArticleHtml> GetLatest(int articleId);
    }
}