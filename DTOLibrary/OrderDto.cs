namespace DTOLibrary;

public class OrderDto
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.Now;
  
    public int UserOderNr { get; set; }
  
    public int UserId { get; set; }

    public int OrderStateId { get; set; }
  
    public virtual OrderStateDto OrderState { get; set; } = null!;
}