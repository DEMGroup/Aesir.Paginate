using Aesir.Paginate.Contracts;
using Aesir.Paginate.Models;

namespace Aesir.Paginate.Extensions;

public static class EnumerableExtensions
{
	public static async Task<PagedResult<T>> ToPagedAsync<T>(
			this IEnumerable<T> source,
			IPaginated config,
			CancellationToken cancellationToken
	) => await source.AsQueryable().ToPagedAsync(config, cancellationToken);
}
