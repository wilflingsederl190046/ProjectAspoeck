using BreakfastDBLib;

namespace ProjectAspoeck.Models;

public class Place_OrderModel
{
  public string SessionString { get; set; } = null!;
    public List<Place_OrderViewModel> orderItems { get; set; }
}
