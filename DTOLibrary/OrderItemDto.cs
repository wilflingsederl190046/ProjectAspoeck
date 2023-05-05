namespace DTOLibrary;

public class OrderItemDto
{
    public int OrderItemId { get; set; }

    public double Price { get; set; }

    public int Quantity { get; set; }

    public int OrderId { get; set; }

    public int ItemId { get; set; }

    public  ItemDto Item { get; set; } = null!;

    public  OrderDto Order { get; set; } = null!;
    public byte[] ImageData { get; set; } = null!;
}