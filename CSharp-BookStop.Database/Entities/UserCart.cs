namespace CSharp_BookStop.Database.Entities;

public class UserCart
{
    public Guid UserCartId { get; set; }
    public required Guid UserId { get; set; }
    
    public IEnumerable<UserCartItem>? UserCartItems { get; set; }
}