using System.Linq.Expressions;
using Aesir.Paginate.Filtering;
using Aesir.Paginate.Filtering.Models;
using Aesir.Paginate.Models;
using Aesir.Paginate.Sorting;

namespace Aesir.Paginate.Extensions;

public static class EnumerableExtensions
{
    private static PagedCollectionResponse<T> ToPagedResponse<T>(
        this IEnumerable<T> source,
        Uri route,
        int pageNumber,
        int pageSize,
        IReadOnlyCollection<string> filterBy,
        IReadOnlyCollection<string> orderBy
    ) where T : class
    {
        var enumerable = source as T[] ?? source.ToArray();
        return new PagedCollectionResponse<T>(
            enumerable.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
            route,
            enumerable.Length,
            pageNumber,
            pageSize,
            filterBy,
            orderBy
        );
    }
    
    public static PagedCollectionResponse<T> PageAndSortData<T>(
        this IEnumerable<T> source,
        Uri baseUrl,
        DataFilterBase filter,
        Expression<Func<T, bool>>? predicate = null
    )
        where T : class
    {
        if (predicate != null)
            source = source.Where(predicate.Compile());

        return source
            .Filter(filter.FilterBy)
            .Sort(filter.OrderBy)
            .ToPagedResponse(
                baseUrl,
                filter.PageNumber,
                filter.PageSize,
                filter.FilterBy,
                filter.OrderBy
            );
    }

    private static IEnumerable<T> Filter<T>(this IEnumerable<T> source, List<string> filters) where T : class
        => FilterProvider.Filter(source, filters);

    private static IEnumerable<T> Sort<T>(
        this IEnumerable<T> items,
        IEnumerable<string> orderBy
    ) where T : class => SortProvider.Sort(items, orderBy);
}