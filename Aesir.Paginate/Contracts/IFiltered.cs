using Aesir.Paginate.Enums;
using JetBrains.Annotations;

namespace Aesir.Paginate.Contracts;

[PublicAPI]
public interface IFiltered
{
	string? FilteredProperty { get; }
	string? Value { get; }
	FilterType Type { get; }
}