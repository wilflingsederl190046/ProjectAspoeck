using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProjectAspoeck.Models;

public class Place_OrderViewModel
{
  public byte[]? ImageUrl { get; set; }
  public string Name { get; set; } = null!;
  public double Price { get; set; }
  public int? Units { get; set; }
}
