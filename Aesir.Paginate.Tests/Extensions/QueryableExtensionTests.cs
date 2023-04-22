using Aesir.Paginate.Extensions;
using Aesir.Paginate.Filtering.Models;
using Aesir.Paginate.Tests.Mocks;

namespace Aesir.Paginate.Tests.Extensions;

public class QueryableExtensionTests
{
    private static readonly Order[] Orders =
    {
        new() { Id = 2, SortId = 1, OrderNumber = "100", OrderType = OrderType.Invoice },
        new() { Id = 1, SortId = 2, OrderNumber = "50", OrderType = OrderType.Cash },
        new() { Id = 3, SortId = 3, OrderNumber = "60", OrderType = OrderType.Invoice },
        new() { Id = 4, SortId = 4, OrderNumber = "75", OrderType = OrderType.Cash },
        new() { Id = 5, SortId = 5, OrderNumber = "125", OrderType = OrderType.Invoice }
    };

    private static TestContext CreateMockContext()
    {
        var db = new TestContext();
        db.Customers.Add(new Customer
        {
            Id = 1,
            Name = "Testy McTestFace",
            Orders = Orders
        });
        db.SaveChanges();
        return db;
    }
    
    [Fact]
    public void PageAndSortData_WithValidArguments_ReturnsPagedData()
    {
        var db = CreateMockContext();
        var filter = new DataFilterBase
        {
            PageNumber = 1,
            PageSize = 30,
            MaxPageSize = 30
        };
        var uri = new Uri("https://www.example.com");
        var result = db.Orders.PageAndSortData(uri, filter);

        Assert.Equal(5, result.TotalRecords);
        Assert.Equal(2, result.Items.ElementAt(0).Id);
        Assert.Equal(1, result.Items.ElementAt(1).Id);
    }

    [Fact]
    public void PageAndSortData_WithValidArguments_ReturnsPagedAndSortedData()
    {
        var db = CreateMockContext();
        var filter = new DataFilterBase
        {
            PageNumber = 1,
            PageSize = 30,
            MaxPageSize = 30,
            OrderBy =
            {
                "Id"
            }
        };
        var uri = new Uri("https://www.example.com");
        var result = db.Orders.PageAndSortData(uri, filter);
    
        Assert.Equal(5, result.TotalRecords);
        Assert.Equal(1, result.Items.ElementAt(0).Id);
        Assert.Equal(2, result.Items.ElementAt(1).Id);
    }
    
    [Fact]
    public void PageAndSortData_WithValidArguments_ReturnsPagedAndSortedData_2Pages()
    {
        var db = CreateMockContext();
        var filter = new DataFilterBase
        {
            PageNumber = 1,
            PageSize = 3,
            MaxPageSize = 30,
            OrderBy =
            {
                "Id"
            }
        };
        var uri = new Uri("https://www.example.com");
        var result = db.Orders.PageAndSortData(uri, filter);
    
        Assert.Equal(5, result.TotalRecords);
        Assert.Equal(2, result.TotalPages);
        Assert.Equal(1, result.Items.ElementAt(0).Id);
        Assert.Equal(2, result.Items.ElementAt(1).Id);
    }
    
    [Fact]
    public void PageAndSortData_WithValidArguments_ReturnsPagedFilteredAndSortedData()
    {
        var db = CreateMockContext();
        var filter = new DataFilterBase
        {
            PageNumber = 1,
            PageSize = 30,
            MaxPageSize = 30,
            OrderBy =
            {
                "Id"
            },
            FilterBy =
            {
                "OrderNumber:5"
            }
        };
        var uri = new Uri("https://www.example.com");
        var result = db.Orders.PageAndSortData(uri, filter);
    
        Assert.Equal(3, result.TotalRecords);
        Assert.Equal(1, result.Items.ElementAt(0).Id);
        Assert.Equal(4, result.Items.ElementAt(1).Id);
    }
    
    [Fact]
    public void PageAndSortData_WithSelectedData_ReturnsPagedFilteredAndSortedData()
    {
        var db = CreateMockContext();
        var filter = new DataFilterBase
        {
            PageNumber = 1,
            PageSize = 30,
            MaxPageSize = 30,
            OrderBy =
            {
                "Id"
            },
            FilterBy =
            {
                "OrderNumber:5"
            }
        };
        var uri = new Uri("https://www.example.com");
        var result = db.Orders.Select(x => new
        {
            x.Id,
            x.OrderNumber
        }).PageAndSortData(uri, filter);
    
        Assert.Equal(3, result.TotalRecords);
        Assert.Equal(1, result.Items.ElementAt(0).Id);
        Assert.Equal(4, result.Items.ElementAt(1).Id);
    }
    
    [Fact]
    public void PageAndSortData_WithPredicatedData_ReturnsPagedFilteredAndSortedData()
    {
        var db = CreateMockContext();
        var filter = new DataFilterBase
        {
            PageNumber = 1,
            PageSize = 30,
            MaxPageSize = 30,
            OrderBy =
            {
                "Id"
            },
            FilterBy =
            {
                "OrderNumber:5"
            }
        };
        var uri = new Uri("https://www.example.com");
        var result = db.Orders.PageAndSortData(uri, filter, x => x.Id != 4);
    
        Assert.Equal(2, result.TotalRecords);
        Assert.Equal(1, result.Items.ElementAt(0).Id);
        Assert.Equal(5, result.Items.ElementAt(1).Id);
    }
}