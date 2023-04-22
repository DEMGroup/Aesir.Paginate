using Aesir.Paginate.Utils;

namespace Aesir.Paginate.Models;

public class PagedCollectionResponse<T>
    where T : class
{
    public IEnumerable<T> Items { get; set; }
    public string? NextPage { get; }
    public string? PreviousPage { get; }
    public string FirstPage { get; }
    public string LastPage { get; }
    public int TotalPages { get; }
    public int TotalRecords { get; }
    public int CurrentPage { get; }
    public int PageSize { get; }

    public PagedCollectionResponse(
        IEnumerable<T> items,
        Uri baseUrl,
        int totalCount,
        int pageNum,
        int pageSize,
        IReadOnlyCollection<string>? filteredBy = null,
        IReadOnlyCollection<string>? orderedBy = null
    )
    {
        Items = items;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        TotalRecords = totalCount;
        CurrentPage = pageNum;
        PageSize = pageSize;
       
        NextPage =
            CurrentPage < TotalPages
                ? CreatePageUrl(baseUrl, CurrentPage + 1, filteredBy, orderedBy)
                : null;
        PreviousPage =
            CurrentPage > 1 ? CreatePageUrl(baseUrl, CurrentPage - 1, filteredBy, orderedBy) : null;
        FirstPage = CreatePageUrl(baseUrl, 1, filteredBy, orderedBy);
        LastPage = CreatePageUrl(baseUrl, TotalPages, filteredBy, orderedBy);
    }

    private string CreatePageUrl(
        Uri baseUrl,
        int pageNum,
        IReadOnlyCollection<string>? filteredBy,
        IReadOnlyCollection<string>? orderedBy
    )
    {
        var page = UriHelpers.GetPageUri(baseUrl, pageNum, PageSize);
        if (filteredBy != null && filteredBy.Count != 0)
            page = UriHelpers.AddQueryParamArray(page, "filterBy", filteredBy);
        if (orderedBy != null && orderedBy.Count != 0)
            page = UriHelpers.AddQueryParamArray(page, "orderBy", orderedBy);
        return page.ToString();
    }
}