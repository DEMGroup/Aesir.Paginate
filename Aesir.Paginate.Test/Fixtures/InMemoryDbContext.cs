using Aesir.Paginate.Test.Fixtures.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aesir.Paginate.Test.Fixtures;

public class InMemoryDbContext : DbContext
{
	public DbSet<User> Users { get; set; }
	public DbSet<Order> Orders { get; set; }
	public DbSet<UserOrder> UserOrders { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
			optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<User>().HasKey(b => b.Id);

		modelBuilder.Entity<Order>().HasKey(b => b.Id);

		modelBuilder.Entity<UserOrder>().HasKey(b => b.Id);

		modelBuilder
				.Entity<UserOrder>()
				.HasOne(uo => uo.User)
				.WithMany(u => u.UserOrders)
				.HasForeignKey(uo => uo.UserId);

		modelBuilder
				.Entity<UserOrder>()
				.HasOne(uo => uo.Order)
				.WithMany()
				.HasForeignKey(uo => uo.OrderId);
	}
}
