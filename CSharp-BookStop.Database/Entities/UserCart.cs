namespace CSharp_BookStop.Database.Entities;

public class UserCart
{
    public Guid UserCartId { get; set; }
    public required Guid UserId { get; set; }
    
    public ICollection<UserCartItem>? UserCartItems { get; set; }
}