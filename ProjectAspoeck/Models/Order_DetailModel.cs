namespace ProjectAspoeck.Models;

public class Order_DetailModel
{
    public byte[]? ImageUrl { get; set; }
    public string ItemName { get; set; } = null!;
    public double ItemPrice { get; set; }
    public string UserName { get; set; }
  
}
