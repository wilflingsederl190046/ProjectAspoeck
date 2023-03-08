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
            User user = _db.Users.Where(m => m.UserName == _loginModel.LoginId && m.ChipNumber == _loginModel.Password).FirstOrDefault();
            if (user == null)
            {
                ViewBag.LoginStatus = 0;
                return View(_loginModel);
            }
            else
            {
                // Generate session key
                string sessionKey = Guid.NewGuid().ToString();

                // Encrypt username and password using session key
                string encryptedUsername = EncryptionHelper.Encrypt(_loginModel.LoginId, sessionKey);
                string encryptedPassword = EncryptionHelper.Encrypt(_loginModel.Password, sessionKey);

                // Store session key and encrypted username and password in session
                HttpContext.Session.SetString("SessionKey", sessionKey);
                HttpContext.Session.SetString("EncryptedUsername", encryptedUsername);
                HttpContext.Session.SetString("EncryptedPassword", encryptedPassword);

                // Redirect to home page with session key in query string
                return RedirectToAction("Home_Page", "Home", new { sessionKey = sessionKey });
            }
        }

        public IActionResult Home_Page(string sessionKey)
        {
            // Retrieve encrypted username and password from session
            string encryptedUsername = HttpContext.Session.GetString("EncryptedUsername");
            string encryptedPassword = HttpContext.Session.GetString("EncryptedPassword");

            // Decrypt username and password using session key
            string username = EncryptionHelper.Decrypt(encryptedUsername, sessionKey);
            string password = EncryptionHelper.Decrypt(encryptedPassword, sessionKey);

            // Retrieve user from database using decrypted username and password
            var homeModel = new Home_PageModel();
            homeModel.UserName = username;
            //homeModel.UserId = loginModel.UserId;

            var orders = new List<OrderViewModel>
    {
        new OrderViewModel { OrderNumber = 1, OrderDate = "Mo, 01.01.2023", OrderAmount = 130.00m, IsPaid = true },
        new OrderViewModel { OrderNumber = 2, OrderDate = "Di, 15.02.2023", OrderAmount = 72.50m, IsPaid = false },
        new OrderViewModel { OrderNumber = 3, OrderDate = "Mi, 28.02.2023", OrderAmount = 42.00m, IsPaid = false }
    };
            homeModel.orders = orders;

            return View(homeModel);
        }
        public IActionResult Settings(SettingsModel settingsModel)
        {
            SettingsModel settings= new SettingsModel();
            return View(settings);
        }

        public IActionResult All_Orders(All_OrdersModel all_OrdersModel)
        {
            All_OrdersModel all_Orders = new All_OrdersModel();
            return View(all_Orders);
        }
        public IActionResult Order_Detail(Order_DetailModel order_DetailModel)
        {
            Order_DetailModel order_Detail = new Order_DetailModel();
            return View(order_Detail);
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
