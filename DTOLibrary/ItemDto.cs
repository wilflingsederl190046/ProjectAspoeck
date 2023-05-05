namespace DTOLibrary;

public class ItemDto
{
    public int ItemId { get; set; }

    public string Name { get; set; } = null!;

    public bool Active { get; set; } = true;

    public double Price { get; set; }
  
    public int? Weekday { get; set; }
  
    public int? ImageId { get; set; }

    public byte[] ImageData { get; set; } = null!;
}