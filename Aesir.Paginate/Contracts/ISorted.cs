namespace Aesir.Paginate.Contracts;

public interface ISorted
{
	string? SortedProperty { get; }
	bool IsAscending { get; }
}