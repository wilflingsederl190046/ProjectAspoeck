using BreakfastDBLib;

namespace ProjectAspoeck.Models;

public class Shopping_BasketModel
{
  public string sessionString { get; set; } = null!;
    public List<Item> OrderItems { get; set; }


   

}
