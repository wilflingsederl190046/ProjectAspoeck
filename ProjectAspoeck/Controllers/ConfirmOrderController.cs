namespace ProjectAspoeck.Controllers;

public class ConfirmOrderController: Controller
{
    private readonly BreakfastDBContext _db = new();

    [HttpPost]
    public IActionResult ConfirmOrder()
    {
        
        return RedirectToAction("Index", "Home");
    }
}