using BreakfastDBLib;

using DTOLibary;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectAspoeck.Models;

using System.Diagnostics;
using System.Drawing;

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
            //string password = EncryptionHelper.Decrypt(encryptedPassword, sessionKey);
            User user = _db.Users.Where(x => x.UserName == username).FirstOrDefault();

            var homeModel = new Home_PageModel();
            homeModel.UserName = username;
            int userid = user.UserId;
            var orders1 = _db.Orders.Include(x=>x.User).Where(x => x.UserId == userid).Select(x => new OrderViewModel { OrderNumber = 0, State = x.OrderState.Name , OrderDate = x.OrderDate.ToString("d") , OrderAmount = x.OrderItems.Count }).ToList();
            
            if (orders1.Count == 0)
            {
                orders1 = new List<OrderViewModel>
            {
                new OrderViewModel { OrderNumber = -1, OrderDate = "", OrderAmount = -1, State = "" },            
            };
            }
            
            homeModel.orders = orders1;


            homeModel.sessionString= sessionKey;
            return View(homeModel);
            
        }
        public IActionResult Settings(string sessionKey)
        {
            
           string encryptedUsername = HttpContext.Session.GetString("EncryptedUsername");
            string username = EncryptionHelper.Decrypt(encryptedUsername, sessionKey);

            User user = _db.Users.Where(x => x.UserName == username).FirstOrDefault();
            
            SettingsModel settings = new SettingsModel();
            settings.Email = user.Email;
            return View(settings);
        }

        public IActionResult All_Orders(string sessionKey)
        {
            
            
            string encryptedUsername = HttpContext.Session.GetString("EncryptedUsername");
            string username = EncryptionHelper.Decrypt(encryptedUsername, sessionKey);
            User user = _db.Users.Where(x => x.UserName == username).FirstOrDefault();

            All_OrdersModel all_Orders = new All_OrdersModel();
            all_Orders.sessionString =sessionKey;
            return View(all_Orders);
        }
        public IActionResult Order_Detail(string sessionKey)
        {
            Order_DetailModel order_Detail = new Order_DetailModel();
            order_Detail.sessionString = sessionKey;
            return View(order_Detail);
        }

        public IActionResult Privacy()
        {
           return View();
        }
        [HttpGet]
        public IActionResult LoginWithChip()
        {
            LoginModel _loginModel = new LoginModel();
            return View(_loginModel);
        }

        [HttpPost]
        public IActionResult LoginWithChip(LoginModel _loginModel)
        {

            User user = _db.Users.Where(m => m.ChipNumber == _loginModel.Password).FirstOrDefault();
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
                string encryptedUsername = EncryptionHelper.Encrypt(user.UserName, sessionKey);
                string encryptedPassword = EncryptionHelper.Encrypt(_loginModel.Password, sessionKey);

                // Store session key and encrypted username and password in session
                HttpContext.Session.SetString("SessionKey", sessionKey);
                HttpContext.Session.SetString("EncryptedUsername", encryptedUsername);
                HttpContext.Session.SetString("EncryptedPassword", encryptedPassword);

                // Redirect to home page with session key in query string
                return RedirectToAction("Home_Page", "Home", new { sessionKey = sessionKey });
            }
        }


        public IActionResult Place_Order(string sessionKey)
        {
            Place_OrderModel place_Order = new Place_OrderModel();
            place_Order.sessionString = sessionKey;
            return View(place_Order);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
