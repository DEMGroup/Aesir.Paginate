using Aesir.Paginate.Contracts;
using Aesir.Paginate.Enums;
using Aesir.Paginate.Extensions;
using Aesir.Paginate.Test.Fixtures;
using Aesir.Paginate.Test.Fixtures.Entities;
using Aesir.Paginate.Test.Fixtures.Generators;

namespace Aesir.Paginate.Test.Extensions;

public class QueryableExtensionsTests
{
	[Fact]
	public async Task ToPaged_ReturnsPaged()
	{
		await using var db = await CreateDatabase();
		var pagination = new Pagination(20, 1);

		var paged = await db.UserOrders.ToPagedAsync(pagination, default);
		Assert.Equal(500, paged.TotalRecords);
		Assert.Equal(pagination.PerPage, paged.RecordsPerPage);
		Assert.Equal(pagination.Page, paged.CurrentPage);
		Assert.Equal(1, paged.Records.First().Id);
	}

	[Fact]
	public async Task ToPaged_ReturnsPaged_DynamicObject()
	{
		await using var db = await CreateDatabase();
		var pagination = new Pagination(20, 1);

		var paged = await db.Orders.Select(x => new { x.Id }).ToPagedAsync(pagination, default);
		Assert.Equal(500, paged.TotalRecords);
		Assert.Equal(pagination.PerPage, paged.RecordsPerPage);
		Assert.Equal(pagination.Page, paged.CurrentPage);
		Assert.Equal(1, paged.Records.First().Id);
	}

	[Fact]
	public async Task ToPaged_ReturnsPaged_AndFiltered_DynamicObject()
	{
		await using var db = await CreateDatabase();

		var pagination = new PagedAndFiltered(20, 1, "Id", "1", FilterType.Contains);

		var paged = await db.Orders.Select(x => new { x.Id }).ToPagedAsync(pagination, default);

		Assert.Equal(176, paged.TotalRecords);
		Assert.Equal(pagination.PerPage, paged.RecordsPerPage);
		Assert.Equal(pagination.Page, paged.CurrentPage);
		Assert.Equal(1, paged.Records.First().Id);
	}

	[Fact]
	public async Task ToPaged_ReturnsPaged_AndFiltered_Joined()
	{
		await using var db = await CreateDatabase();

		var pagination = new PagedAndFiltered(20, 1, "Order.Id", "1", FilterType.Contains);

		var paged = await db
				.Orders.Join(
						db.UserOrders,
						o => o.UserOrderId,
						uo => uo.Id,
						(o, uo) => new { Order = o, uo.UserId }
				)
				.ToPagedAsync(pagination, default);

		Assert.Equal(176, paged.TotalRecords);
		Assert.Equal(pagination.PerPage, paged.RecordsPerPage);
		Assert.Equal(pagination.Page, paged.CurrentPage);
		Assert.Equal(1, paged.Records.First().Order.Id);
	}

	[Fact]
	public async Task ToPaged_ReturnsPaged_AndFiltered_Contains()
	{
		await using var db = await CreateDatabase();

		var pagination = new PagedAndFiltered(
				20,
				1,
				nameof(UserOrder.OrderId),
				"1",
				FilterType.Contains
		);

		var paged = await db.UserOrders.ToPagedAsync(pagination, default);
		Assert.Equal(176, paged.TotalRecords);
		Assert.Equal(pagination.PerPage, paged.RecordsPerPage);
		Assert.Equal(pagination.Page, paged.CurrentPage);
		Assert.Equal(1, paged.Records.First().Id);
	}

	[Fact]
	public async Task ToPaged_ReturnsPaged_AndFiltered_StartsWith()
	{
		await using var db = await CreateDatabase();

		var pagination = new PagedAndFiltered(
				20,
				1,
				nameof(UserOrder.OrderId),
				"1",
				FilterType.StartsWith
		);

		var paged = await db.UserOrders.ToPagedAsync(pagination, default);
		Assert.Equal(111, paged.TotalRecords);
		Assert.Equal(pagination.PerPage, paged.RecordsPerPage);
		Assert.Equal(pagination.Page, paged.CurrentPage);
		Assert.Equal(1, paged.Records.First().Id);
	}

	[Fact]
	public async Task ToPaged_ReturnsPaged_AndFiltered_EndsWith()
	{
		await using var db = await CreateDatabase();
		var pagination = new PagedAndFiltered(
				20,
				1,
				nameof(UserOrder.OrderId),
				"1",
				FilterType.EndsWith
		);

		var paged = await db.UserOrders.ToPagedAsync(pagination, default);
		Assert.Equal(50, paged.TotalRecords);
		Assert.Equal(pagination.PerPage, paged.RecordsPerPage);
		Assert.Equal(pagination.Page, paged.CurrentPage);
		Assert.Equal(1, paged.Records.First().Id);
	}

	[Fact]
	public async Task ToPaged_ReturnsPaged_AndSorted()
	{
		await using var db = await CreateDatabase();
		var pagination = new PagedAndSorted(20, 1, "Id", false);

		var paged = await db.UserOrders.ToPagedAsync(pagination, default);
		Assert.Equal(500, paged.TotalRecords);
		Assert.Equal(pagination.PerPage, paged.RecordsPerPage);
		Assert.Equal(pagination.Page, paged.CurrentPage);
		Assert.Equal(500, paged.Records.First().Id);
	}

	[Fact]
	public async Task ToPaged_ReturnsPaged_AndSorted_DynamicObject()
	{
		await using var db = await CreateDatabase();
		var pagination = new PagedAndSorted(20, 1, "OrderId", false);

		var paged = await db
				.UserOrders.Select(x => new
				{
					x.UserId,
					x.OrderId,
					x.Id,
				})
				.ToPagedAsync(pagination, default);
		Assert.Equal(500, paged.TotalRecords);
		Assert.Equal(pagination.PerPage, paged.RecordsPerPage);
		Assert.Equal(pagination.Page, paged.CurrentPage);
		Assert.Equal(500, paged.Records.First().Id);
	}

	[Fact]
	public async Task ToPaged_ReturnsPaged_AndSorted_Joined()
	{
		await using var db = await CreateDatabase();
		var pagination = new PagedAndSorted(20, 1, "Order.Id", false);

		var paged = await db
				.Orders.Join(
						db.UserOrders,
						o => o.UserOrderId,
						uo => uo.Id,
						(o, uo) => new { Order = o, uo.UserId }
				)
				.ToPagedAsync(pagination, default);

		Assert.Equal(500, paged.TotalRecords);
		Assert.Equal(pagination.PerPage, paged.RecordsPerPage);
		Assert.Equal(pagination.Page, paged.CurrentPage);
		Assert.Equal(500, paged.Records.First().Order.Id);
	}

	[Fact]
	public async Task ToPaged_ReturnsPaged_AndSorted_AndFiltered()
	{
		await using var db = await CreateDatabase();
		var pagination = new PagedAndFilteredAndSorted(
				20,
				1,
				"Id",
				"1",
				FilterType.Contains,
				"Id",
				false
		);

		var paged = await db.UserOrders.ToPagedAsync(pagination, default);
		Assert.Equal(176, paged.TotalRecords);
		Assert.Equal(pagination.PerPage, paged.RecordsPerPage);
		Assert.Equal(pagination.Page, paged.CurrentPage);
		Assert.Equal(491, paged.Records.First().Id);
	}

	[Fact]
	public async Task ToPaged_ReturnsPaged_AndSorted_AndFiltered_DynamicObject()
	{
		await using var db = await CreateDatabase();
		var pagination = new PagedAndFilteredAndSorted(
				20,
				1,
				"Id",
				"1",
				FilterType.Contains,
				"Id",
				false
		);

		var paged = await db
				.UserOrders.Select(x => new
				{
					x.UserId,
					x.OrderId,
					x.Id,
				})
				.ToPagedAsync(pagination, default);
		Assert.Equal(176, paged.TotalRecords);
		Assert.Equal(pagination.PerPage, paged.RecordsPerPage);
		Assert.Equal(pagination.Page, paged.CurrentPage);
		Assert.Equal(491, paged.Records.First().Id);
	}

	[Fact]
	public async Task ToPaged_ReturnsPaged_AndSorted_AndFiltered_Joined()
	{
		await using var db = await CreateDatabase();
		var pagination = new PagedAndFilteredAndSorted(
				20,
				1,
				"Order.Id",
				"1",
				FilterType.Contains,
				"Order.Id",
				false
		);

		var paged = await db
				.Orders.Join(
						db.UserOrders,
						o => o.UserOrderId,
						uo => uo.Id,
						(o, uo) => new { Order = o, uo.UserId }
				)
				.ToPagedAsync(pagination, default);

		Assert.Equal(176, paged.TotalRecords);
		Assert.Equal(pagination.PerPage, paged.RecordsPerPage);
		Assert.Equal(pagination.Page, paged.CurrentPage);
		Assert.Equal(491, paged.Records.First().Order.Id);
	}

	private static async Task<InMemoryDbContext> CreateDatabase()
	{
		var db = new InMemoryDbContext();
		var user = new User { Age = 21, Name = "Test" };
		var orders = OrderGenerator.Generate(500, 1).ToArray();
		db.Users.Add(user);
		db.Orders.AddRange(orders.Select(x => x.Item1));
		db.UserOrders.AddRange(orders.Select(x => x.Item2));
		await db.SaveChangesAsync();
		return db;
	}

	private record Pagination(int? PerPage, int? Page) : IPaginated;

	private record PagedAndSorted(int? PerPage, int? Page, string SortedProperty, bool? IsAscending)
			: IPaginated,
					ISorted;

	private record PagedAndFiltered(
			int? PerPage,
			int? Page,
			string FilteredProperty,
			string Value,
			FilterType? Type
	) : IPaginated, IFiltered;

	private record PagedAndFilteredAndSorted(
			int? PerPage,
			int? Page,
			string FilteredProperty,
			string Value,
			FilterType? Type,
			string SortedProperty,
			bool? IsAscending
	) : IPaginated, IFiltered, ISorted;
}
