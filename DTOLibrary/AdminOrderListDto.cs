namespace DTOLibrary;

public class AdminOrderListDto
{
    public int OrderNumber { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = null!;
    public string UserName { get; set; }
    public string State { get; set; } = null!;
    public double Price { get; set; }
}
