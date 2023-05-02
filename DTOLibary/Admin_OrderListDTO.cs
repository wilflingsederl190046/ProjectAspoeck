namespace DTOLibary;

public class Admin_OrderListDTO
{
  public int OrderNumber { get; set; }
  public DateTime Date { get; set; }
  public string Description { get; set; } = null!;
  public string State { get; set; } = null!;
  public double Price { get; set; }
}
