namespace CSharp_BookStop.Database.Entities;

public class UserCartItem
{
    public required Guid UserCartItemId { get; set; }
    
    public required int Quantity { get; set; }
    public required Book Book { get; set; }
}