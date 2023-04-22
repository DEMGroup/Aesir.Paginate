using System.Linq.Expressions;
using Aesir.Paginate.Filtering;
using Aesir.Paginate.Filtering.Models;
using Aesir.Paginate.Models;
using Aesir.Paginate.Sorting;

namespace Aesir.Paginate.Extensions;

public static class QueryableExtensions
{
    private static PagedCollectionResponse<T> ToPagedResponse<T>(
        this IQueryable<T> source,
        Uri route,
        int pageNumber,
        int pageSize,
        IReadOnlyCollection<string> filterBy,
        IReadOnlyCollection<string> orderBy
    ) where T : class
        => new(
            source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
            route,
            source.Count(),
            pageNumber,
            pageSize,
            filterBy,
            orderBy
        );

    public static PagedCollectionResponse<T> PageAndSortData<T>(
        this IQueryable<T> source,
        Uri baseUrl,
        DataFilterBase filter,
        Expression<Func<T, bool>>? predicate = null
    )
        where T : class
    {
        if (predicate != null)
            source = source.Where(predicate);

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

    private static IQueryable<T> Filter<T>(this IQueryable<T> source, IEnumerable<string> filters) where T : class
        => FilterProvider.Filter(source, filters);

    private static IQueryable<T> Sort<T>(
        this IQueryable<T> items,
        IEnumerable<string> orderBy
    ) where T : class => SortProvider.Sort(items, orderBy);
}