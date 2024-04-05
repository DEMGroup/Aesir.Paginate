namespace Aesir.Paginate.Test.Fixtures.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public ICollection<UserOrder> UserOrders { get; set; }
}