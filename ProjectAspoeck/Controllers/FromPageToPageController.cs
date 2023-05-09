namespace ProjectAspoeck.Controllers;

public class FromPageToPageController: Controller
{
    private readonly BreakfastDBContext _db = new();

    public void SetFromPageToPage(string fromPage,string fromController, HttpContext context)
    {
     

            context.Session.SetString("fromPage", fromPage);
            context.Session.SetString("fromPage", fromController);
        
    }

    [HttpPost]
    public IActionResult BckToLastPage(HttpContext context)
    {

        string fromPage = context.Session.GetString("fromPage");
        string fromController = context.Session.GetString("fromPage");
        
        return RedirectToAction(fromPage, fromController);
    }
    
}