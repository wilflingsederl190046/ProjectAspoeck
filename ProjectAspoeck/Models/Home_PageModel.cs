namespace ProjectAspoeck.Models;

public class Home_PageModel
{
  public int UserId { get; set; }
  public string? UserName { get; set; }
  public List<OrderViewModel> Orders { get; set; }
  public string SessionString { get; set; } = null!;
}
