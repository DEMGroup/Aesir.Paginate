using Aesir.Paginate.Sorting;
using Aesir.Paginate.Tests.Mocks;

namespace Aesir.Paginate.Tests.Sorting;

public class SortProviderTests
{
    private record SortableItem(int Id, string Name, int Age = 0);

    private static readonly Order[] Orders =
    {
        new() { Id = 2, SortId = 1, OrderNumber = "100", OrderType = OrderType.Invoice },
        new() { Id = 1, SortId = 2, OrderNumber = "50", OrderType = OrderType.Cash },
        new() { Id = 3, SortId = 3, OrderNumber = "60", OrderType = OrderType.Invoice }
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
    public void EnumerableSort_ShouldSortAListBySingleProperty()
    {
        var myList = new List<SortableItem>
        {
            new(1, "John"),
            new(2, "Alice"),
            new(3, "Bob")
        };
        var orderBy = new List<string> { "Name" };

        var sortedList = SortProvider.Sort(myList, orderBy).ToList();

        Assert.Equal("Alice", sortedList[0].Name);
        Assert.Equal("Bob", sortedList[1].Name);
        Assert.Equal("John", sortedList[2].Name);
    }

    [Fact]
    public void QueryableSort_ShouldSortAListBySingleProperty()
    {
        var orderBy = new List<string> { "Id" };

        var db = CreateMockContext();
        var sortedList = SortProvider.Sort(db.Orders, orderBy).ToList();

        Assert.Equal(1, sortedList[0].Id);
        Assert.Equal(2, sortedList[1].Id);
        Assert.Equal(3, sortedList[2].Id);
    }

    [Fact]
    public void EnumerableSort_ShouldSortAListBySinglePropertyDescending()
    {
        var myList = new List<SortableItem>
        {
            new(1, "John"),
            new(2, "Alice"),
            new(3, "Bob")
        };
        var orderBy = new List<string> { "Name,desc" };
        var sortedList = SortProvider.Sort(myList, orderBy).ToList();

        Assert.Equal("John", sortedList[0].Name);
        Assert.Equal("Bob", sortedList[1].Name);
        Assert.Equal("Alice", sortedList[2].Name);
    }

    [Fact]
    public void QueryableSort_ShouldSortAListBySinglePropertyDescending()
    {
        var orderBy = new List<string> { "Id,desc" };

        var db = CreateMockContext();
        var sortedList = SortProvider.Sort(db.Orders, orderBy).ToList();

        Assert.Equal(3, sortedList[0].Id);
        Assert.Equal(2, sortedList[1].Id);
        Assert.Equal(1, sortedList[2].Id);
    }

    [Fact]
    public void EnumerableSort_ShouldSortAListByMultipleProperties()
    {
        var myList = new List<SortableItem>
        {
            new(1, "John", 30),
            new(2, "Alice", 25),
            new(3, "Bob", 35),
            new(1, "Alice", 30)
        };
        var orderBy = new List<string> { "Name", "Age" };
        var sortedList = SortProvider.Sort(myList, orderBy).ToList();

        Assert.Equal("Alice", sortedList[0].Name);
        Assert.Equal(25, sortedList[0].Age);
        Assert.Equal("Alice", sortedList[1].Name);
        Assert.Equal(30, sortedList[1].Age);
        Assert.Equal("Bob", sortedList[2].Name);
        Assert.Equal(35, sortedList[2].Age);
        Assert.Equal("John", sortedList[3].Name);
        Assert.Equal(30, sortedList[3].Age);
    }

    [Fact]
    public void QueryableSort_ShouldSortAListByMultipleProperties()
    {
        var orderBy = new List<string> { "SortId", "OrderType" };

        var db = new TestContext();
        db.Customers.Add(new Customer
        {
            Id = 1,
            Name = "Testy McTestFace",
            Orders = Orders.Concat(new[] { new Order { Id = 4, SortId = 1, OrderNumber = "100", OrderType = OrderType.Cash} }).ToArray()
        });
        db.SaveChanges();
        var sortedList = SortProvider.Sort(db.Orders, orderBy).ToList();

        Assert.Equal(4, sortedList[0].Id);
        Assert.Equal(OrderType.Cash, sortedList[0].OrderType);
        Assert.Equal(2, sortedList[1].Id);
        Assert.Equal(OrderType.Invoice, sortedList[1].OrderType);
        Assert.Equal(1, sortedList[2].Id);
        Assert.Equal(OrderType.Cash, sortedList[2].OrderType);
        Assert.Equal(3, sortedList[3].Id);
        Assert.Equal(OrderType.Invoice, sortedList[3].OrderType);
    }
    
    [Fact]
    public void EnumerableSort_ShouldErrorOnMissingProperties()
    {
        var myList = new List<SortableItem>
        {
            new(1, "John", 30),
            new(2, "Alice", 25),
            new(3, "Bob", 35),
            new(1, "Alice", 30)
        };
        var orderBy = new List<string> { "Name", "Car" };
        Assert.Throws<ArgumentException>(() => SortProvider.Sort(myList, orderBy).ToList());
    }

    [Fact]
    public void QueryableSort_ShouldErrorOnMissingProperties()
    {
        var orderBy = new List<string> { "SortId", "Cart" };
    
        var db = new TestContext();
        db.Customers.Add(new Customer
        {
            Id = 1,
            Name = "Testy McTestFace",
            Orders = Orders.Concat(new[] { new Order { Id = 4, SortId = 1, OrderNumber = "100", OrderType = OrderType.Cash} }).ToArray()
        });
        db.SaveChanges();
        Assert.Throws<ArgumentException>(() =>  SortProvider.Sort(db.Orders, orderBy).ToList());
    }
}