namespace ProjectAspoeck.Controllers;

public class FromPageToPageController: Controller
{
    private readonly BreakfastDBContext _db = new();
    
    public void SetFromPageToPage(string fromPage, string fromController, HttpContext context)
    {
        context.Session.SetString("fromPage1", fromPage);
        context.Session.SetString("fromController1", fromController);
    }

    [HttpPost]
    public IActionResult BackToLastPage()
    {

        string fromPage = HttpContext.Session.GetString("fromPage1");
        string fromController = HttpContext.Session.GetString("fromController1");
        
        return RedirectToAction(fromPage, fromController);
        
    }
    
}