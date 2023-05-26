namespace ProjectAspoeck.Models.User;

public class HomePageModel
{
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public List<OrderViewModel> Orders { get; set; }
    public string SessionString { get; set; } = null!;
    public double MoneyLeftToPay { get; set; }
    public bool AlreadyOrderedToday { get; set; }
}