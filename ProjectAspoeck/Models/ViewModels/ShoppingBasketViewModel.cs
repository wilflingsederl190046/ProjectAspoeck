namespace ProjectAspoeck.Models;

public class ShoppingBasketViewModel
{
  public byte[]? ImageUrl { get; set; }
  public string Name { get; set; } = null!;
  public double Price { get; set; }
  public int Quantity { get; set; }
  public double Total { get; set; }
}
