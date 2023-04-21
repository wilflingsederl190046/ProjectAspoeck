namespace ProjectAspoeck.Models;

public class Place_OrderViewModel
{
  public byte[] ImageUrl { get; set; }
  public string Bezeichnung { get; set; } = null;
  public string Kosten { get; set; }
  public int? Units { get; set; }
}
