using System.Linq.Expressions;
using Aesir.Paginate.Contracts;
using Aesir.Paginate.Enums;

namespace Aesir.Paginate.Filtering;

internal static class PredicateBuilder
{
	internal static Expression<Func<T, bool>>? BuildPredicate<T>(IFiltered filter)
	{
		if (filter.FilteredProperty is null)
			return null;

		var parameter = Expression.Parameter(typeof(T), "x");
		var properties = filter.FilteredProperty.Split('.');

		if (!HasProperty(parameter.Type, properties[0]))
			throw new ArgumentException(
					$"{properties[0]} does not exist on type {parameter.Type.Name}."
			);

		Expression propertyAccess = Expression.Property(parameter, properties[0]);

		foreach (var property in properties.Skip(1))
		{
			if (!HasProperty(propertyAccess.Type, property))
				throw new ArgumentException(
						$"{property} does not exist on type {propertyAccess.Type.Name}."
				);

			propertyAccess = Expression.Property(propertyAccess, property);
		}

		if (propertyAccess.Type != typeof(string))
		{
			if (!HasToString(propertyAccess.Type))
				throw new ArgumentException($"Cannot convert property to a string for searching.");
			propertyAccess = Expression.Call(propertyAccess, "ToString", null);
		}

		Expression body = filter.Type switch
		{
			FilterType.StartsWith => Expression.Call(
					propertyAccess,
					typeof(string).GetMethod("StartsWith", [typeof(string)])!,
					Expression.Constant(filter.Value)
			),
			FilterType.EndsWith => Expression.Call(
					propertyAccess,
					typeof(string).GetMethod("EndsWith", [typeof(string)])!,
					Expression.Constant(filter.Value)
			),
			FilterType.Contains => Expression.Call(
					propertyAccess,
					typeof(string).GetMethod("Contains", [typeof(string)])!,
					Expression.Constant(filter.Value)
			),
			_ => throw new ArgumentException("Unsupported filter type."),
		};

		return Expression.Lambda<Func<T, bool>>(body, parameter);
	}

	internal static (Expression, Type) CreateKeySelector<T>(string path)
	{
		var parameter = Expression.Parameter(typeof(T), "x");
		var properties = path.Split('.');

		if (!HasProperty(parameter.Type, properties[0]))
			throw new ArgumentException(
					$"{properties[0]} does not exist on type {parameter.Type.Name}."
			);
		Expression propertyAccess = Expression.Property(parameter, properties[0]);

		foreach (var property in properties.Skip(1))
		{
			if (!HasProperty(propertyAccess.Type, property))
				throw new ArgumentException(
						$"{property} does not exist on type {propertyAccess.Type.Name}."
				);

			propertyAccess = Expression.Property(propertyAccess, property);
		}

		return (Expression.Lambda(propertyAccess, parameter), propertyAccess.Type);
	}

	private static bool HasProperty(Type type, string propertyName) =>
			type.GetProperty(propertyName) != null;

	private static bool HasToString(Type type) =>
			type.GetMethods().FirstOrDefault(x => x.Name == "ToString") != null;
}
