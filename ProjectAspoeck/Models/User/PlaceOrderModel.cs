namespace ProjectAspoeck.Models.User;

public class PlaceOrderModel
{
  public List<PlaceOrderViewModel> OrderItems { get; set; }
  public string PlusButton { get; set; }
  public string SessionString { get; set; } = null!;
}
