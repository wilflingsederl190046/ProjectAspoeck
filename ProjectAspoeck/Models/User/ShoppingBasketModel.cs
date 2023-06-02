namespace ProjectAspoeck.Models.User;

public class ShoppingBasketModel
{
  public string SessionString { get; set; } = null!;
  public List<OrderItemDto> OrderItems { get; set; }
}
