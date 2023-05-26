namespace ProjectAspoeck.Models.User;

public class OrderDetailModel
{
    public List<PlaceOrderViewModel> Order { get; set; } = null!;
    public double TotalPrice { get; set; }
    public DateTime OrderDate { get; set; }
    public string UserName { get; set; }
    public int TotalItemCount { get; set; }
    
    
    
}
