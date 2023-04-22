using Microsoft.EntityFrameworkCore;

namespace Aesir.Paginate.Tests.Mocks;

public class TestContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Customer>()
            .HasMany(c => c.Orders)
            .WithOne(o => o.Customer)
            .HasForeignKey(o => o.CustomerId);
    }
}

public class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

public class Order
{
    public int Id { get; set; }
    public OrderType OrderType { get; set; }
    public required string OrderNumber { get; set; }
    public int SortId { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}

public enum OrderType
{
    Cash,
    Invoice
}