using System.Linq.Expressions;
using Aesir.Paginate.Contracts;
using Aesir.Paginate.Filtering;
using Aesir.Paginate.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Aesir.Paginate.Extensions;

[PublicAPI]
public static class QueryableExtensions
{
    public static async Task<PagedResult<T>> ToPagedAsync<T>(
        this IQueryable<T> source,
        IPaginated config,
        CancellationToken cancellationToken
    )
    {
        source = config is IFiltered filtered && IsFiltering(filtered) ? source.ToFiltered(filtered) : source;
        source = config is ISorted sorted && IsSorting(sorted) ? source.ToSorted(sorted) : source;

        var totalRecords = source.Count();
        var page = config.Page ?? 1;
        var perPage = config.PerPage ?? 20;

        source = source
            .Skip((page - 1) * perPage)
            .Take(perPage);

         var items = source is IAsyncEnumerable<T>
            ? await source.ToArrayAsync(cancellationToken)
            : source.ToArray();

        return new PagedResult<T>(
            totalRecords,
            page,
            config.PerPage ?? 20,
            items
        );
    }

    private static IQueryable<T> ToFiltered<T>(
        this IQueryable<T> source,
        IFiltered filter
    ) => PredicateBuilder.BuildPredicate<T>(filter) is { } expr ? source.Where(expr) : source;

    private static IQueryable<T> ToSorted<T>(
        this IQueryable<T> source,
        ISorted filter
    )
    {
        if (filter.SortedProperty is null) return source;

        var (selector, propertyType) = PredicateBuilder.CreateKeySelector<T>(filter.SortedProperty);
        var orderByMethod = filter.IsAscending
            ? Expression.Call(typeof(Queryable), "OrderBy", [typeof(T), propertyType], source.Expression, selector)
            : Expression.Call(typeof(Queryable), "OrderByDescending", [typeof(T), propertyType], source.Expression,
                selector);

        return source.Provider.CreateQuery<T>(orderByMethod);
    }

    private static bool IsFiltering(IFiltered filter)
        => !string.IsNullOrEmpty(filter.FilteredProperty) && !string.IsNullOrEmpty(filter.Value);

    private static bool IsSorting(ISorted sort)
        => !string.IsNullOrEmpty(sort.SortedProperty);
}