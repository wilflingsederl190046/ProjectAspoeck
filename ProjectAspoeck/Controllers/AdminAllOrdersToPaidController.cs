namespace ProjectAspoeck.Controllers;

public class AdminAllOrdersToPaidController : Controller
{
    private readonly BreakfastDBContext _db = new();

    [HttpPost]
    public IActionResult AdminAllOrdersToPaid()
    {
        
        string sessionKey = "notAuthorized";
        sessionKey = HttpContext.Session.GetString("SessionKey")?? sessionKey;
        if (sessionKey == "notAuthorized")
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            
        }

        return RedirectToAction("");
    }
}