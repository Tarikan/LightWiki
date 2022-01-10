using System.Collections.Generic;
using System.Threading.Tasks;
using LightWiki.Data.Mongo.Models;

namespace LightWiki.Data.Mongo.Repositories
{
    public interface IBaseRepository<T> where T : BaseModel
    {
        Task<List<T>> Get();

        Task<T> Get(string id);

        Task<T> Create(T entity);

        Task Update(string id, T entityIn);

        Task Remove(T entityIn);

        Task Remove(string id);
    }
}