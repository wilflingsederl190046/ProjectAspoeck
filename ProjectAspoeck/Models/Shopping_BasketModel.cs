using BreakfastDBLib;

namespace ProjectAspoeck.Models;

public class Shopping_BasketModel
{
  public string sessionString { get; set; } = null!;
    public List<OrderItem> OrderItems { get; set; }
}
