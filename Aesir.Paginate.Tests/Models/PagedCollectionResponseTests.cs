using Aesir.Paginate.Models;

namespace Aesir.Paginate.Tests.Models;

public class PagedCollectionResponseTests
{
    [Fact]
    public void PagedCollectionResponse_TotalPagesIsCorrect()
    {
        var items = new List<string> { "item1", "item2", "item3" };
        const int totalCount = 10;
        const int pageSize = 3;
        const int pageNum = 1;
        var baseUrl = new Uri("https://example.com");

        var pagedCollection = new PagedCollectionResponse<string>(items, baseUrl, totalCount, pageNum, pageSize);

        Assert.Equal(4, pagedCollection.TotalPages);
    }

    [Fact]
    public void PagedCollectionResponse_NextPageIsCorrect()
    {
        var items = new List<string> { "item1", "item2", "item3" };
        const int totalCount = 10;
        const int pageSize = 3;
        const int pageNum = 1;
        var baseUrl = new Uri("https://example.com");

        var pagedCollection = new PagedCollectionResponse<string>(items, baseUrl, totalCount, pageNum, pageSize);

        Assert.Equal("https://example.com/?pageNumber=2&pageSize=3", pagedCollection.NextPage);
    }

    [Fact]
    public void PagedCollectionResponse_PreviousPageIsCorrect()
    {
        var items = new List<string> { "item1", "item2", "item3" };
        const int totalCount = 10;
        const int pageSize = 3;
        const int pageNum = 2;
        var baseUrl = new Uri("https://example.com");

        var pagedCollection = new PagedCollectionResponse<string>(items, baseUrl, totalCount, pageNum, pageSize);

        Assert.Equal("https://example.com/?pageNumber=1&pageSize=3", pagedCollection.PreviousPage);
    }

    [Fact]
    public void PagedCollectionResponse_FirstPageIsCorrect()
    {
        var items = new List<string> { "item1", "item2", "item3" };
        const int totalCount = 10;
        const int pageSize = 3;
        const int pageNum = 2;
        var baseUrl = new Uri("https://example.com");

        var pagedCollection = new PagedCollectionResponse<string>(items, baseUrl, totalCount, pageNum, pageSize);

        Assert.Equal("https://example.com/?pageNumber=1&pageSize=3", pagedCollection.FirstPage);
    }

    [Fact]
    public void PagedCollectionResponse_LastPageIsCorrect()
    {
        var items = new List<string> { "item1", "item2", "item3" };
        const int totalCount = 10;
        const int pageSize = 3;
        const int pageNum = 2;
        var baseUrl = new Uri("https://example.com");

        var pagedCollection = new PagedCollectionResponse<string>(items, baseUrl, totalCount, pageNum, pageSize);

        Assert.Equal("https://example.com/?pageNumber=4&pageSize=3", pagedCollection.LastPage);
    }

    [Fact]
    public void NextPage_ReturnsNull_WhenCurrentPageEqualsTotalPages()
    {
        var items = new List<string> { "item1", "item2", "item3" };
        var baseUrl = new Uri("https://example.com");
        const int totalCount = 3;
        const int pageNum = 3;
        const int pageSize = 1;
        var filteredBy = new List<string> { "filter1" };
        var orderedBy = new List<string> { "order1" };

        var response = new PagedCollectionResponse<string>(
            items,
            baseUrl,
            totalCount,
            pageNum,
            pageSize,
            filteredBy,
            orderedBy
        );

        Assert.Null(response.NextPage);
    }
    
    [Fact]
    public void PreviousPage_ReturnsNull_WhenCurrentPageIsOne()
    {
        var items = new List<string> { "item1", "item2", "item3" };
        var baseUrl = new Uri("https://example.com");
        const int totalCount = 3;
        const int pageNum = 1;
        const int pageSize = 1;
        var filteredBy = new List<string> { "filter1" };
        var orderedBy = new List<string> { "order1" };
    
        var response = new PagedCollectionResponse<string>(
            items,
            baseUrl,
            totalCount,
            pageNum,
            pageSize,
            filteredBy,
            orderedBy
        );

        Assert.Null(response.PreviousPage);
    }
    
    [Fact]
    public void TotalPages_CalculatesCorrectly()
    {
        var items = new List<string> { "item1", "item2", "item3" };
        var baseUrl = new Uri("https://example.com");
        const int totalCount = 5;
        const int pageNum = 1;
        const int pageSize = 2;
        var filteredBy = new List<string> { "filter1" };
        var orderedBy = new List<string> { "order1" };
    
        var response = new PagedCollectionResponse<string>(
            items,
            baseUrl,
            totalCount,
            pageNum,
            pageSize,
            filteredBy,
            orderedBy
        );

        Assert.Equal(3, response.TotalPages);
    }
    
    [Fact]
    public void PagedCollectionResponse_TotalRecordsIsCorrect()
    {
        var items = new List<string> { "item1", "item2", "item3" };
        const int totalCount = 3;
        const int pageSize = 3;
        const int pageNum = 2;
        var baseUrl = new Uri("https://example.com");

        var pagedCollection = new PagedCollectionResponse<string>(items, baseUrl, totalCount, pageNum, pageSize);

        Assert.Equal(3, pagedCollection.TotalRecords);
    }
    
    [Fact]
    public void PagedCollectionResponse_ItemsIsCorrect()
    {
        var items = new List<string> { "item1", "item2", "item3" };
        const int totalCount = 3;
        const int pageSize = 3;
        const int pageNum = 2;
        var baseUrl = new Uri("https://example.com");

        var pagedCollection = new PagedCollectionResponse<string>(items, baseUrl, totalCount, pageNum, pageSize);

        Assert.Equal(items, pagedCollection.Items);
    }
}