namespace ProjectAspoeck.Models.Admin;

public class EditListModelModel
{
    public string SessionKey { get; set; }
    public List<ItemDto> Items   { get; set; }
    
    public List<ImageDto> Images { get; set; }
}