using BreakfastDBLib;

using DTOLibary;

using Microsoft.AspNetCore.Mvc;

using ProjectAspoeck.Models;

using System.Diagnostics;

namespace ProjectAspoeck.Controllers
{

  public class HomeController : Controller
  {
    BreakfastContext _db = new BreakfastContext();
    private readonly ILogger<HomeController> _logger;
    public HomeController(ILogger<HomeController> logger)
    {
      _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
      LoginModel _loginModel = new LoginModel();
      return View(_loginModel);
    }
    [HttpPost]
    public IActionResult Index(LoginModel _loginModel)
    {

      List<UserDTO> _users = _db.Users.Select(x => new UserDTO { UserId = x.UserId, LastName = x.LastName, FirstName = x.FirstName }).ToList();
      Console.WriteLine(_users);

      User? user = _db.Users.Where(m => m.UserName == _loginModel.LoginId && m.ChipNumber == _loginModel.Password).FirstOrDefault();
      if (user == null)
      {

        ViewBag.LoginStatus = 0;
      }
      else
      {
        _loginModel.UserId = user.UserId;
        var loginModel = new LoginModel
        {
          UserId = _loginModel.UserId,
          // add other necessary properties here
        };
        return RedirectToAction("Home_Page", "Home", loginModel);
      }
      return View(_loginModel);
    }

    public IActionResult Home_Page(LoginModel loginModel)
    {

      User user = _db.Users.Where(x => x.UserId == loginModel.UserId).FirstOrDefault();
      string name = user.UserName ?? "UserName";
      var homeModel = new Home_PageModel();
      homeModel.UserName = name;
      homeModel.UserId = loginModel.UserId;
      var orders = new List<OrderViewModel>
             {
                new OrderViewModel { OrderNumber = 1, OrderDate = "Mo, 01.01.2023", OrderAmount = 130.00m, IsPaid = true },
                new OrderViewModel { OrderNumber = 2, OrderDate = "Di, 15.02.2023", OrderAmount = 72.50m, IsPaid = false },
                new OrderViewModel { OrderNumber = 3, OrderDate = "Mi, 28.02.2023", OrderAmount = 42.00m, IsPaid = false }
            };

      homeModel.orders = orders;
      return View(homeModel);
        }
        public IActionResult Settings(Settings settingsModel)
        {
            Settings settings= new Settings();
            return View(settings);
        }
        public IActionResult Privacy()
        {
         return View();
         }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
