using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using LinqKit;

namespace Aesir.Paginate.Filtering;

public static class FilterProvider
{
    public static IEnumerable<T> Filter<T>(IEnumerable<T> source, IEnumerable<string> filters)
        => Create<T>(filters) is { } predicate ? source.Where(predicate.Compile()) : source;

    public static IQueryable<T> Filter<T>(IQueryable<T> source, IEnumerable<string> filters)
        => Create<T>(filters) is { } predicate ? source.Where(predicate) : source;

    //Input item: col:[item1,item2] or col:item1
    private static ExpressionStarter<T>? Create<T>(IEnumerable<string> filters)
    {
        if (!filters.Any()) return null;
        return filters
            .Select(filter => filter.Split(':'))
            .Where(split => split.Length == 2)
            .Select(split => GetProperty<T>(split[0]) is { } prop ? BuildPredicate<T>(prop, split[1]) : null)
            .Where(subPredicate => subPredicate != null)
            .Aggregate(PredicateBuilder.New<T>(), (current, subPredicate) => current.And(subPredicate));
    }

    private static PropertyInfo? GetProperty<T>(string filter)
    {
        var propertyInfos = typeof(T).GetProperties(
            BindingFlags.Public | BindingFlags.Instance
        );

        return Array.Find(
            propertyInfos,
            pi => pi.Name.Equals(filter, StringComparison.InvariantCultureIgnoreCase)
        );
    }

    private static ExpressionStarter<T>? BuildPredicate<T>(PropertyInfo prop, string filter)
    {
        var arr = filter
            .Replace("[", "")
            .Replace("]", "")
            .Split(',')
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrEmpty(x))
            .ToArray();

        if (arr.Length == 0)
            return null;

        return arr.Aggregate(
            PredicateBuilder.New<T>(),
            (current, item) => current.Or(ColumnContains<T>(prop, item ?? ""))
        );
    }

    private static Expression<Func<T, bool>>? ColumnContains<T>(PropertyInfo prop, string search)
    {
        var obj = Expression.Parameter(typeof(T), "obj");
        var objProperty = Expression.Property(obj, prop);
        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
        var searchConst = Expression.Constant(search.ToLower());

        if (objProperty.Type.IsEnum)
        {
            var isInt = int.TryParse(search.ToLower(), out var intVal);
            if (!isInt)
                return null;

            var val = Expression.Constant(Enum.ToObject(objProperty.Type, intVal));
            var exp = Expression.Equal(objProperty, val);
            return Expression.Lambda<Func<T, bool>>(exp, obj);
        }

        if (objProperty.Type == typeof(string))
        {
            var toLowerMethod = typeof(string).GetMethod("ToLower", Array.Empty<Type>());
            var memberToLowerCall = Expression.Call(objProperty, toLowerMethod!);
            var nullCheck = Expression.NotEqual(objProperty, Expression.Constant(null));
            var containsCall = Expression.Call(memberToLowerCall, containsMethod, searchConst);
            var conditionalExpression = Expression.Condition(nullCheck, containsCall, Expression.Constant(false));
            return Expression.Lambda<Func<T, bool>>(conditionalExpression, obj);
        }

        var toStringMethod = typeof(object).GetMethod("ToString")!;
        var memberToStringCall = Expression.Call(objProperty, toStringMethod);
        var toLowerMethod2 = typeof(string).GetMethod("ToLower", Array.Empty<Type>());
        var memberToStringToLowerCall = Expression.Call(memberToStringCall, toLowerMethod2!);
        var call = Expression.Call(memberToStringToLowerCall, containsMethod, searchConst);
        return Expression.Lambda<Func<T, bool>>(call, obj);
    }
}