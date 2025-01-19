using Aesir.Paginate.Test.Fixtures.Entities;

namespace Aesir.Paginate.Test.Fixtures.Generators;

internal static class OrderGenerator
{
	internal static IEnumerable<(Order, UserOrder)> Generate(int count, int userId) =>
			Enumerable
					.Range(1, count)
					.Select(x =>
							(
									new Order
									{
										Id = x,
										ProductName = $"Product {x}",
										Price = x * 2,
										UserOrderId = x,
									},
									new UserOrder
									{
										Id = x,
										UserId = userId,
										OrderId = x,
									}
							)
					);
}
