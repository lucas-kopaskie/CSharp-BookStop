namespace CSharp_BookStop.API.Models;

public class UserCart
{
    public required int UserCartId { get; set; }
    public required int UserId { get; set; }
    
    public IEnumerable<UserCartItem>? UserCartItems { get; set; }
}