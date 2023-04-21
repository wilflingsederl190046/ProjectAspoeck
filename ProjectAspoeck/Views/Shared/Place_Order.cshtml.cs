using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JausenbestellungAspoeck.Pages;

public class Place_Order : PageModel
{
  private readonly BreakfastDBContext _db;

  public Place_Order(BreakfastDBContext db) => _db = db;

  public string ImageUrlPageModel { get; set; }

  public async Task<IActionResult> OnGetAsync(int id)
  {
    // Suchen Sie das Bild in der Datenbank anhand der Id
    var image = await _db.Images.FindAsync(id);

    // Wenn das Bild nicht gefunden wird, geben Sie einen 404-Fehler zurück
    if (image == null)
    {
      return NotFound();
    }

    // Wandeln Sie die Binärdaten des Bildes in einen Base64-String um
    string base64 = Convert.ToBase64String(image.ImageData);

    // Setzen Sie die URL des Bildes auf den Base64-String
    ImageUrlPageModel = $"data:image/png;base64,{base64}";

    return Page();
  }
}
