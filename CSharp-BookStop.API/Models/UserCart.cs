using System.ComponentModel.DataAnnotations.Schema;

namespace CSharp_BookStop.API.Models;

public class UserCart
{
    public Guid UserCartId { get; set; }
    public required Guid UserId { get; set; }
    
    public IEnumerable<UserCartItem>? UserCartItems { get; set; }
}