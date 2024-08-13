using JetBrains.Annotations;

namespace Aesir.Paginate.Contracts;

[PublicAPI]
public interface IPaginated
{
    int? PerPage { get; }
    int? Page { get; }
}