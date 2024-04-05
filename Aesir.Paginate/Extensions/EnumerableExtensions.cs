using Aesir.Paginate.Contracts;
using Aesir.Paginate.Models;

namespace Aesir.Paginate.Extensions;

public static class EnumerableExtensions
{
    public static PagedResult<T> ToPaged<T>(
        this IEnumerable<T> source,
        IPaginated config
    ) => source.AsQueryable().ToPaged(config);
}