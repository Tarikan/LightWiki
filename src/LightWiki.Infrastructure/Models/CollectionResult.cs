using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LightWiki.Infrastructure.Models;

public class CollectionResult<T>
{
    public CollectionResult(IList<T> collection, int total)
    {
        Collection = new ReadOnlyCollection<T>(collection);
        Total = total;
    }

    public IReadOnlyCollection<T> Collection { get; set; }

    public int Total { get; set; }
}