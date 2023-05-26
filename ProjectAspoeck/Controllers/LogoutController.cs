namespace ProjectAspoeck.Controllers;

public class LogoutController : Controller
{
    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}