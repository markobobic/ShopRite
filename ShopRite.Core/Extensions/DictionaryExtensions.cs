using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Core.Extensions
{
    public static class DictionaryExtensions
    {
        public static TV GetValueOrDefault<TK, TV>(this IDictionary<TK, TV> dict, TK key, TV defaultValue = default(TV)) =>
               dict.TryGetValue(key, out TV value) ? value : defaultValue;
        public static async Task<(IEnumerable<T> PaginatedList, QueryStatistics QueryStatistics)> ToPagination<T>(this IRavenQueryable<T> source, int pageNumber, int limit, CancellationToken cancellationToken) =>
            (await source
                 .Statistics(out QueryStatistics stats)
                 .Skip((pageNumber - 1) * limit)
                 .Take(limit).ToListAsync(cancellationToken), stats);
    }
}
