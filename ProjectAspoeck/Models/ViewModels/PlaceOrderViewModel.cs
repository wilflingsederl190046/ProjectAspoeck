using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProjectAspoeck.Models;

public class PlaceOrderViewModel
{
  public byte[]? ImageUrl { get; set; }
  public string Name { get; set; } = null!;
  public double Price { get; set; }
  public int? Units { get; set; }
}
