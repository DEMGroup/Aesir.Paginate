namespace Aesir.Paginate.Test.Fixtures.Entities;

public class UserOrder
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int OrderId { get; set; }
    public Order? Order { get; set; }
}