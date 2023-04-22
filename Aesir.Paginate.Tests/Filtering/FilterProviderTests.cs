using Aesir.Paginate.Filtering;
using Aesir.Paginate.Tests.Mocks;

namespace Aesir.Paginate.Tests.Filtering;

public class FilterProviderTests
{
    private record FilterableItem(string Property1, string Property2);

    private readonly FilterableItem[] _testArray =
    {
        new("Test1", "Test2"),
        new("Toast1", "Toast2"),
        new("Twist1", "Twist2")
    };

    private static readonly Order[] Orders =
    {
        new() { Id = 1, OrderNumber = "1", OrderType = OrderType.Cash },
        new() { Id = 2, OrderNumber = "2", OrderType = OrderType.Cash },
        new() { Id = 3, OrderNumber = "3", OrderType = OrderType.Invoice }
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
    public void Filter_ShouldNotChange_WhenGivenNoFilters_Enumerable()
    {
        var arr = _testArray.Take(1).ToArray();
        var res = FilterProvider.Filter(arr, Array.Empty<string>());
        Assert.Equal(arr, res);

        arr = _testArray.Take(2).ToArray();
        res = FilterProvider.Filter(arr, Array.Empty<string>());
        Assert.Equal(arr, res);

        arr = _testArray;
        res = FilterProvider.Filter(arr, Array.Empty<string>());
        Assert.Equal(arr, res);
    }

    [Fact]
    public void Filter_ShouldReturnFilteredArray_WhenGivenFilters_Enumerable()
    {
        var res = FilterProvider.Filter(_testArray, new[] { "Property1:Test" });
        Assert.Equal(new[] { _testArray[0] }, res);
    }

    [Fact]
    public void Filter_ShouldReturnFilteredArray_WhenGivenMultipleFilters_Enumerable()
    {
        var res = FilterProvider.Filter(_testArray, new[] { "Property1:[Test,Toast]" });
        Assert.Equal(new[] { _testArray[0], _testArray[1] }, res);
    }

    [Fact]
    public void Filter_ShouldReturnFilteredArray_WhenGivenMultipleFiltersWithSpaces_Enumerable()
    {
        var res = FilterProvider.Filter(_testArray, new[] { "Property1:[    Test      ,    Toast      ]" });
        Assert.Equal(new[] { _testArray[0], _testArray[1] }, res);
    }

    [Fact]
    public void Filter_ShouldStillReturnFilteredArray_WhenGivenIncorrectProps_Enumerable()
    {
        var res = FilterProvider.Filter(_testArray, new[] { "Property1:Test", "Property4:Test" });
        Assert.Equal(new[] { _testArray[0] }, res);
    }

    [Fact]
    public void Filter_ShouldReturnEmptyArray_WhenGivenIncorrectProps_Enumerable()
    {
        var res = FilterProvider.Filter(_testArray, new[] { "Property1:" });
        Assert.Equal(Array.Empty<FilterableItem>(), res);

        res = FilterProvider.Filter(_testArray, new[] { "Property1:[]" });
        Assert.Equal(Array.Empty<FilterableItem>(), res);
    }

    [Fact]
    public void Filter_ShouldStillReturnFilteredArray_WhenMultipleFilters_Enumerable()
    {
        var res = FilterProvider.Filter(_testArray, new[] { "Property1:Test", "Property2:Toast" });
        Assert.Equal(Array.Empty<FilterableItem>(), res);

        res = FilterProvider.Filter(_testArray, new[] { "Property1:Test", "Property2:Test" });
        Assert.Equal(new[] { _testArray[0] }, res);
    }

    [Fact]
    public void Filter_ShouldNotChange_WhenGivenNoFilters_Queryable()
    {
        var db = CreateMockContext();
        var res = FilterProvider.Filter(db.Orders, Array.Empty<string>());
        Assert.Equal(Orders, res);
    }

    [Fact]
    public void Filter_ShouldReturnFilteredArray_WhenGivenFilters_Queryable()
    {
        var db = CreateMockContext();
        var res = FilterProvider.Filter(db.Orders, new[] { "OrderNumber:1" });
        Assert.Equal(new[] { Orders[0] }, res);
    }

    [Fact]
    public void Filter_ShouldReturnFilteredArray_WhenGivenMultipleFilters_Queryable()
    {
        var db = CreateMockContext();
        var res = FilterProvider.Filter(db.Orders, new[] { "OrderNumber:[1,2]" });
        Assert.Equal(new[] { Orders[0], Orders[1] }, res);
    }

    [Fact]
    public void Filter_ShouldReturnFilteredArray_WhenGivenMultipleFiltersWithSpaces_Queryable()
    {
        var db = CreateMockContext();
        var res = FilterProvider.Filter(db.Orders, new[] { "OrderNumber:[    1      ,    2      ]" });
        Assert.Equal(new[] { Orders[0], Orders[1] }, res);
    }

    [Fact]
    public void Filter_ShouldStillReturnFilteredArray_WhenGivenIncorrectProps_Queryable()
    {
        var db = CreateMockContext();
        var res = FilterProvider.Filter(db.Orders, new[] { "OrderNumber:1", "OrderItem:Test" });
        Assert.Equal(new[] { Orders[0] }, res);
    }

    [Fact]
    public void Filter_ShouldStillReturnFilteredArray_WhenMultipleFilters_Queryable()
    {
        var db = CreateMockContext();
        var res = FilterProvider.Filter(db.Orders, new[] { "OrderNumber:1", "Id:5" });
        Assert.Equal(Array.Empty<Order>(), res);

        res = FilterProvider.Filter(db.Orders, new[] { "OrderNumber:1", "Id:1" });
        Assert.Equal(new[] { Orders[0] }, res);
    }

    [Fact]
    public void Filter_ShouldReturnUnchangedArray_WhenGivenIncorrectProps_Queryable()
    {
        var db = CreateMockContext();
        var res = FilterProvider.Filter(db.Orders, new[] { "OrderNumber:" });
        Assert.Equal(Array.Empty<Order>(), res);

        res = FilterProvider.Filter(db.Orders, new[] { "OrderNumber:[]" });
        Assert.Equal(Array.Empty<Order>(), res);
    }

    [Fact]
    public void Filter_ShouldHandleEnums_WhenGivenFilter_Queryable()
    {
        var db = CreateMockContext();
        var res = FilterProvider.Filter(db.Orders, new[] { "OrderType:1" });
        Assert.Equal(new[] { Orders[2] }, res);
    }
    
    [Fact]
    public void Filter_ShouldReturnEmpty_WhenGivenFilter_Queryable()
    {
        var db = CreateMockContext();
        var res = FilterProvider.Filter(db.Orders, new[] { "OrderType:test" });
        Assert.Equal(Array.Empty<Order>(), res);
    }
}