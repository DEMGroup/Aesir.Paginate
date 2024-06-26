using System.Linq.Expressions;
using Aesir.Paginate.Contracts;
using Aesir.Paginate.Filtering;
using Aesir.Paginate.Models;
using JetBrains.Annotations;

namespace Aesir.Paginate.Extensions;

[PublicAPI]
public static class QueryableExtensions
{
    public static PagedResult<T> ToPaged<T>(
        this IQueryable<T> source,
        IPaginated config
    )
    {
        source = config is IFiltered filtered && IsFiltering(filtered) ? source.ToFiltered(filtered) : source;
        source = config is ISorted sorted && IsSorting(sorted) ? source.ToSorted(sorted) : source;
        
        var totalRecords = source.Count();
        return new PagedResult<T>(
            totalRecords,
            config.Page,
            config.PerPage,
            source
                .Skip((config.Page - 1) * config.PerPage)
                .Take(config.PerPage)
        );
    }

    private static IQueryable<T> ToFiltered<T>(
        this IQueryable<T> source,
        IFiltered filter
    ) => source.Where(PredicateBuilder.BuildPredicate<T>(filter));

    private static IQueryable<T> ToSorted<T>(
        this IQueryable<T> source,
        ISorted filter
    )
    {
        var (selector, propertyType) = PredicateBuilder.CreateKeySelector<T>(filter.SortedProperty);
        var orderByMethod = filter.IsAscending
            ? Expression.Call(typeof(Queryable), "OrderBy", new[] { typeof(T), propertyType }, source.Expression, selector)
            : Expression.Call(typeof(Queryable), "OrderByDescending", new[] { typeof(T), propertyType }, source.Expression, selector);

        return source.Provider.CreateQuery<T>(orderByMethod);
    }

    private static bool IsFiltering(IFiltered filter)
        => !string.IsNullOrEmpty(filter.FilteredProperty) && !string.IsNullOrEmpty(filter.Value);

    private static bool IsSorting(ISorted sort)
        => !string.IsNullOrEmpty(sort.SortedProperty);
}