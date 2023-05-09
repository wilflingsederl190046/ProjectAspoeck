namespace DTOLibrary;

public class ImageDto
{
    public int ImageId { get; set; }

    public string Name { get; set; } = null!;
    
    public byte[] ImageData { get; set; }
}