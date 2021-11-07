using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightWiki.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Infrastructure.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<CollectionResult<T>> ToCollectionResult<T>(this IQueryable<T> queryable, int total)
        {
            var collection = await queryable.ToListAsync();
            return new CollectionResult<T>(collection, total);
        }

        public static async Task<CollectionResult<T>>
            ToCollectionResult<T>(this IQueryable<T> queryable, int total, CancellationToken cancellationToken)
        {
            var collection = await queryable.ToListAsync(cancellationToken);
            return new CollectionResult<T>(collection, total);
        }
    }
}