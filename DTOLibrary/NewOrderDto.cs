namespace DTOLibrary;

public class NewOrderDto
{
  public string SessionKey { get; set; } = "-";
  public List<GetOrderItemDto> OrderItems { get; set; } = new();
}
