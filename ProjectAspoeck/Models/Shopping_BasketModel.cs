namespace ProjectAspoeck.Models;

public class Shopping_BasketModel
{
  public string SessionString { get; set; } = null!;
  public List<Item> OrderItems { get; set; }
}
