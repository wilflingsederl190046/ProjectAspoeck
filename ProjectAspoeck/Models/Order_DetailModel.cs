namespace ProjectAspoeck.Models;

public class Order_DetailModel
{
    public List<Place_OrderViewModel> Order { get; set; } = null!;
    public double TotalPrice { get; set; }
    public DateTime OrderDate { get; set; }
    public string UserName { get; set; }
    public int TotalItemCount { get; set; }
    
    
    
}
