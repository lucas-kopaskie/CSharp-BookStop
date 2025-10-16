namespace CSharp_BookStop.Database.Entities;

public class Cart
{
    public Guid CartId { get; set; }
    public required Guid UserId { get; set; }
    
    public ICollection<CartItem>? CartItems { get; set; }
}