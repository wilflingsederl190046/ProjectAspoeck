using Microsoft.AspNetCore.Mvc;

namespace ProjectAspoeck.Controllers;
public class OrderDetailController : Controller
{
  private readonly BreakfastDBContext _db = new();
  private readonly ILogger<OrderDetailController> _logger;

  public OrderDetailController(ILogger<OrderDetailController> logger) => _logger = logger;

  public IActionResult Order_Detail(string sessionKey) => View(new Order_DetailModel { SessionString = sessionKey });

}
