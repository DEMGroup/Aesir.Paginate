using Aesir.Paginate.Contracts;
using Aesir.Paginate.Enums;

namespace Aesir.Paginate.Test.Filtering;

public class PredicateBuilderTests
{
	[Theory]
	[InlineData(null, "value")]
	[InlineData("", "value")]
	[InlineData("Property", null)]
	[InlineData("Property", "")]
	public void BuildPredicate_ReturnsNull_WhenFilteredPropertyOrValueIsNullOrEmpty(
			string filteredProperty,
			string value
	)
	{
		var filter = new TestFiltered(filteredProperty, value, FilterType.Contains);
		var predicate = Aesir.Paginate.Filtering.PredicateBuilder.BuildPredicate<TestEntity>(
				filter
		);
		Assert.Null(predicate);
	}

	private record TestFiltered(string FilteredProperty, string Value, FilterType? Type) : IFiltered;

	private class TestEntity
	{
		public string Property { get; set; } = string.Empty;
	}
}
