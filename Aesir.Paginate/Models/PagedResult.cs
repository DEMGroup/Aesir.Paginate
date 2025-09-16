namespace Aesir.Paginate.Models;

public record PagedResult<T>(
		int TotalRecords,
		int CurrentPage,
		int RecordsPerPage,
		IEnumerable<T> Records
);