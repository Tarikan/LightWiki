using System.Threading.Tasks;
using LightWiki.Data.Mongo.Models;

namespace LightWiki.Data.Mongo.Repositories;

public interface IArticleMdRepository : IBaseRepository<ArticleMd>
{
    Task<ArticleMd> GetLatest(int articleId);
}