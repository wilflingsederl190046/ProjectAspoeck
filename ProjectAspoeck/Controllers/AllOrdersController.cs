using Microsoft.AspNetCore.Mvc;

namespace ProjectAspoeck.Controllers;
public class AllOrdersController : Controller
{
  private readonly BreakfastDBContext _db = new();
  private readonly ILogger<HomeController> _logger;
  public AllOrdersController(ILogger<HomeController> logger) => _logger = logger;

 
}
