using System.Collections.Generic;
using System.Linq;
using Aesir.Paginate.Extensions;
using Aesir.Paginate.Filtering.Models;
using Xunit;

namespace Aesir.Paginate.Tests.Extensions;

public class EnumerableExtensionsTests
{
    private readonly List<MyItem> _testData = new()
    {
        new MyItem { Id = 1, Name = "John", Age = 30 },
        new MyItem { Id = 2, Name = "Jane", Age = 25 },
        new MyItem { Id = 3, Name = "Bob", Age = 40 },
        new MyItem { Id = 4, Name = "Alice", Age = 20 },
        new MyItem { Id = 5, Name = "Charlie", Age = 35 }
    };

    [Fact]
    public void PageAndSortData_WithValidArguments_ReturnsPagedData()
    {
        var filter = new DataFilterBase
        {
            PageNumber = 1,
            PageSize = 30,
            MaxPageSize = 30
        };
        var uri = new Uri("https://www.example.com");
        var result = _testData.PageAndSortData(uri, filter);

        Assert.Equal(5, result.TotalRecords);
        Assert.Equal("John", result.Items.ElementAt(0).Name);
        Assert.Equal("Jane", result.Items.ElementAt(1).Name);
    }

    [Fact]
    public void PageAndSortData_WithValidArguments_ReturnsPagedAndSortedData()
    {
        var filter = new DataFilterBase
        {
            PageNumber = 1,
            PageSize = 30,
            MaxPageSize = 30,
            OrderBy =
            {
                "Name"
            }
        };
        var uri = new Uri("https://www.example.com");
        var result = _testData.PageAndSortData(uri, filter);

        Assert.Equal(5, result.TotalRecords);
        Assert.Equal("Alice", result.Items.ElementAt(0).Name);
        Assert.Equal("Bob", result.Items.ElementAt(1).Name);
    }

    [Fact]
    public void PageAndSortData_WithValidArguments_ReturnsPagedAndSortedData_2Pages()
    {
        var filter = new DataFilterBase
        {
            PageNumber = 1,
            PageSize = 3,
            MaxPageSize = 30,
            OrderBy =
            {
                "Name"
            }
        };
        var uri = new Uri("https://www.example.com");
        var result = _testData.PageAndSortData(uri, filter);

        Assert.Equal(5, result.TotalRecords);
        Assert.Equal(2, result.TotalPages);
        Assert.Equal("Alice", result.Items.ElementAt(0).Name);
        Assert.Equal("Bob", result.Items.ElementAt(1).Name);
    }

    [Fact]
    public void PageAndSortData_WithValidArguments_ReturnsPagedFilteredAndSortedData()
    {
        var filter = new DataFilterBase
        {
            PageNumber = 1,
            PageSize = 30,
            MaxPageSize = 30,
            OrderBy =
            {
                "Name"
            },
            FilterBy =
            {
                "Name:a"
            }
        };
        var uri = new Uri("https://www.example.com");
        var result = _testData.PageAndSortData(uri, filter);

        Assert.Equal(3, result.TotalRecords);
        Assert.Equal("Alice", result.Items.ElementAt(0).Name);
        Assert.Equal("Charlie", result.Items.ElementAt(1).Name);
    }

    [Fact]
    public void PageAndSortData_WithSelectedData_ReturnsPagedFilteredAndSortedData()
    {
        var filter = new DataFilterBase
        {
            PageNumber = 1,
            PageSize = 30,
            MaxPageSize = 30,
            OrderBy =
            {
                "Name"
            },
            FilterBy =
            {
                "Name:a"
            }
        };
        var uri = new Uri("https://www.example.com");
        var result = _testData.Select(x => new
        {
            x.Name
        }).PageAndSortData(uri, filter);

        Assert.Equal(3, result.TotalRecords);
        Assert.Equal("Alice", result.Items.ElementAt(0).Name);
        Assert.Equal("Charlie", result.Items.ElementAt(1).Name);
    }

    [Fact]
    public void PageAndSortData_WithPredicatedData_ReturnsPagedFilteredAndSortedData()
    {
        var filter = new DataFilterBase
        {
            PageNumber = 1,
            PageSize = 30,
            MaxPageSize = 30,
            OrderBy =
            {
                "Name"
            },
            FilterBy =
            {
                "Name:a"
            }
        };
        var uri = new Uri("https://www.example.com");
        var result = _testData.PageAndSortData(uri, filter, x => x.Id != 4);

        Assert.Equal(2, result.TotalRecords);
        Assert.Equal("Charlie", result.Items.ElementAt(0).Name);
        Assert.Equal("Jane", result.Items.ElementAt(1).Name);
    }
}

public class MyItem
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Age { get; set; }
}