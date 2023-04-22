using System.Linq.Expressions;
using System.Reflection;

namespace Aesir.Paginate.Sorting;

internal static class SortProvider
{
    //ordering looks like "property,desc" for descending or "property" for ascending
    public static IQueryable<T> Sort<T>(IQueryable<T> source, IEnumerable<string> orderBy)
    {
        var count = 0;
        foreach (var (descending, orderByClause) in CreateOrderBy<T>(orderBy))
        {
            if (count == 0)
            {
                source = descending
                    ? source.OrderByDescending(orderByClause)
                    : source.OrderBy(orderByClause);
            }
            else
            {
                source = descending
                    ? (source as IOrderedQueryable<T>)!.ThenByDescending(orderByClause)
                    : (source as IOrderedQueryable<T>)!.ThenBy(orderByClause);
            }

            count++;
        }

        return source;
    }
    
    public static IEnumerable<T> Sort<T>(IEnumerable<T> source, IEnumerable<string> orderBy)
    {
        var count = 0;
        foreach (var (descending, orderByClause) in CreateOrderBy<T>(orderBy))
        {
            if (count == 0)
            {
                source = descending
                    ? source.OrderByDescending(orderByClause.Compile())
                    : source.OrderBy(orderByClause.Compile());
            }
            else
            {
                source = descending
                    ? (source as IOrderedEnumerable<T>)!.ThenByDescending(orderByClause.Compile())
                    : (source as IOrderedEnumerable<T>)!.ThenBy(orderByClause.Compile());
            }

            count++;
        }

        return source;
    }

    private static IEnumerable<(bool, Expression<Func<T, object>>)> CreateOrderBy<T>(IEnumerable<string> orderBy)
    {
        var entityType = typeof(T);
        var parameter = Expression.Parameter(entityType, "x");

        var res = new List<(bool, Expression<Func<T, object>>)>(); // descending = true
        foreach (var orderByClause in orderBy)
        {
            var orderByParts = orderByClause.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var propertyName = orderByParts[0];
            var descending = orderByParts.Length == 2 &&
                             orderByParts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

            var propertyInfo = entityType.GetProperty(propertyName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (propertyInfo == null)
            {
                throw new ArgumentException($"Invalid property name: {propertyName}");
            }

            var propertyExpression = Expression.Property(parameter, propertyInfo);
            var lambdaExpression = Expression.Lambda<Func<T, object>>(
                Expression.Convert(propertyExpression, typeof(object)), parameter);

            res.Add((descending, lambdaExpression));
        }

        return res;
    }
}