using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProjectAspoeck.Models;

public class Place_OrderModel
{
  public List<Place_OrderViewModel> OrderItems { get; set; }
  public string PlusButton { get; set; }
  public string SessionString { get; set; } = null!;
}
