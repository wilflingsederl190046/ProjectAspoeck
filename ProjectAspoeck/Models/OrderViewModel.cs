namespace ProjectAspoeck.Models;

public class OrderViewModel
{
  public int OrderNumber { get; set; }
  public string OrderDate { get; set; } = null!;
  public decimal OrderAmount { get; set; }
  public bool IsPaid { get; set; }
}
