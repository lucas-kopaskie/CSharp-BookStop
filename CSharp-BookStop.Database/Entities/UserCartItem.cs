namespace CSharp_BookStop.Database.Entities;

public class CartItem
{
    public required Guid CartItemId { get; set; }
    
    public required int Quantity { get; set; }
    public required Book Book { get; set; }
}