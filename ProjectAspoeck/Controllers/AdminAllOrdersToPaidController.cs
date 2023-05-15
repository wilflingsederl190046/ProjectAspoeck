namespace ProjectAspoeck.Controllers;

public class AdminAllOrdersToPaidController : Controller
{
    private readonly BreakfastDBContext _db = new();

    [HttpPost]
    public IActionResult AdminAllOrdersToPaid()
    {
        return RedirectToAction("Admin_Home_Page", "Admin");
    }
}