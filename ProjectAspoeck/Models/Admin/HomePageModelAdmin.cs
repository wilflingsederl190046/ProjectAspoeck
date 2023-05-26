namespace ProjectAspoeck.Models.Admin;

public class HomePageModelAdmin
{
  public int UserId { get; set; }
  public string? UserName { get; set; }
  public string SessionString { get; set; } = null!;
  public List<AdminOrderListDto> Orders { get; set; }
}
