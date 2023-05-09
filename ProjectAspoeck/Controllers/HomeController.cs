namespace ProjectAspoeck.Controllers;

public class HomeController : Controller
{
    private readonly BreakfastDBContext _db = new();
    private readonly ILogger<HomeController> _logger;
    public HomeController(ILogger<HomeController> logger) => _logger = logger;

    [HttpGet]
    public IActionResult Index() => View(new LoginModel());

    [HttpPost]
    public IActionResult Index(LoginModel loginModel)
    {
        var user = _db.Users.Where(m => m.UserName == loginModel.LoginId).FirstOrDefault();

        if (user == null || !user.VerifyPassword(loginModel.Password))
        {
            ViewBag.LoginStatus = 0;
            return View(loginModel);
        }

        string sessionKey = Guid.NewGuid().ToString();
        if (user.UserName.Equals("Admin"))
        {
            SaveDataInSession(loginModel, sessionKey);
            return RedirectToAction("Admin_Home_Page", "Admin");
        }

        SaveDataInSession(loginModel, sessionKey);

        // Redirect to home page with session key in query string
        return RedirectToAction("Home_Page", "Home" );
    }

    private void SaveDataInSession(LoginModel loginModel, string sessionKey)
    {
        string encryptedUsernameAdmin = EncryptionHelper.Encrypt(loginModel.LoginId, sessionKey);
        string encryptedPasswordAdmin = EncryptionHelper.Encrypt(loginModel.Password, sessionKey);

        // Store session key and encrypted username and password in session
        HttpContext.Session.SetString("SessionKey", sessionKey);
        HttpContext.Session.SetString("EncryptedUsername", encryptedUsernameAdmin);
        HttpContext.Session.SetString("EncryptedPassword", encryptedPasswordAdmin);
    }

    public IActionResult Home_Page()
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
            var fromPage = new FromPageToPageController();
            fromPage.SetFromPageToPage("Home","Home", HttpContext);

            string encryptedUsername = HttpContext.Session.GetString("EncryptedUsername") ?? "";
            string encryptedPassword = HttpContext.Session.GetString("EncryptedPassword") ?? "";

            string username = EncryptionHelper.Decrypt(encryptedUsername, sessionKey);
            User user = _db.Users
                .Where(x => x.UserName == username)
                .FirstOrDefault() ?? new();

            var ordersListForUser = _db.Orders
                .Include(x => x.User)
                .Include(x => x.OrderState)
                .Include(x => x.OrderItems)
                .Where(x => x.UserId == user.UserId)
                .Where(x => x.OrderStateId != 3)
                .Select(x => x)
                .OrderBy(x => x.OrderDate)
                .ToList();

            var orders = ordersListForUser
                .Skip(Math.Max(0, ordersListForUser.Count - 5))
                .OrderByDescending(x => x.OrderDate)
                .Select(x => new OrderViewModel
                {
                    OrderId = x.OrderId,
                    OrderNumber = x.UserOderNr,
                    State = x.OrderState.Name,
                    OrderDate = x.OrderDate.ToString("d"),
                    Price = x.OrderItems.Sum(y => y.Price)
                })
                .ToList();
            if (!orders.Any())
            {
                orders = new List<OrderViewModel>
                {
                    new() { OrderNumber = -1, OrderDate = "", Price = -1, State = "" },
                };
            }

            double sumRestPriceToPay = _db.OrderItems
                .Include(x => x.Order)
                .ThenInclude(x => x.User)
                .Where(x => x.Order.User.UserId == user.UserId)
                .Where(x => x.Order.OrderStateId == 1 || x.Order.OrderStateId == 2)
                .Sum(x => x.Price);
            homeModel = new Home_PageModel
            {
                UserName = username,
                Orders = orders,
                SessionString = sessionKey,
                MoneyLeftToPay = sumRestPriceToPay
            };
            homeModel.SessionString = sessionKey;
            
        }
        
        return View(homeModel);
    }
    
    [HttpGet]
    public IActionResult LoginWithChip() => View(new LoginModel());

    [HttpPost]
    public IActionResult LoginWithChip(LoginModel loginModel)
    {
        User? user = _db.Users
            .Where(m => m.ChipNumber == loginModel.Password)
            .FirstOrDefault();
        if (user == null)
        {
            ViewBag.LoginStatus = 0;
            return View(loginModel);
        }
        // Generate session key
        string sessionKey = Guid.NewGuid().ToString();
        loginModel.LoginId = user.UserName;
        
        SaveDataInSession(loginModel, sessionKey);
        
        if (_db.Orders.Include(x=>x.User).Where(x => x.UserId == user.UserId && x.OrderDate.Date == DateTime.Today && x.OrderStateId == 1 ).Any())
        {
            return RedirectToAction("Order_Detail", "OrderDetail");
        }
        else
        {
            return RedirectToAction("Home_Page", "Home");
        }

        
    }
    
}