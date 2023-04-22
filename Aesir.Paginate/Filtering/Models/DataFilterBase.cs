namespace Aesir.Paginate.Filtering.Models;

public class DataFilterBase
{
    public int MaxPageSize { get; set; } = 100;
    public int PageNumber { get; set; } = 1;
    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
    public List<string> FilterBy { get; } = new();
    public List<string> OrderBy { get; } = new();
}