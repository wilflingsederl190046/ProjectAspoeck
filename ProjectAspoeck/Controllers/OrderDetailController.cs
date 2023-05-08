using Microsoft.AspNetCore.Mvc;

namespace ProjectAspoeck.Controllers;
public class OrderDetailController : Controller
{
  private readonly BreakfastDBContext _db = new();
  private readonly ILogger<OrderDetailController> _logger;

  public OrderDetailController(ILogger<OrderDetailController> logger) => _logger = logger;

  public IActionResult Order_Detail()
  {
    var homeModel = new Home_PageModel();
    string sessionKey = "notAuthorized";
    sessionKey = HttpContext.Session.GetString("SessionKey")?? sessionKey;
    if (sessionKey == "notAuthorized")
    {
      return RedirectToAction("Index", "Home");
    }
    else
    {
      var detailModel = new Order_DetailModel();
      
      
      return View(detailModel);
      
    }
  }
  

  
}
