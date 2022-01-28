using System.Collections.Generic;
using System.Threading.Tasks;
using LightWiki.Data.Mongo.Models;

namespace LightWiki.Data.Mongo.Repositories;

public interface IArticlePatchRepository : IBaseRepository<ArticlePatchModel>
{
    Task<List<ArticlePatchModel>> GetByVersionIds(IList<int> articleVersionIds);
}