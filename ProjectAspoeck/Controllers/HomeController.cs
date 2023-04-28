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
    User? user = _db.Users.Where(m => m.UserName == loginModel.LoginId && m.ChipNumber == loginModel.Password).FirstOrDefault();
    string sessionKey = Guid.NewGuid().ToString();
    if (user.UserName.Equals("Admin"))
    {
      string encryptedUsername = EncryptionHelper.Encrypt(loginModel.LoginId, sessionKey);
      string encryptedPassword = EncryptionHelper.Encrypt(loginModel.Password, sessionKey);

      // Store session key and encrypted username and password in session
      HttpContext.Session.SetString("SessionKey", sessionKey);
      HttpContext.Session.SetString("EncryptedUsername", encryptedUsername);
      HttpContext.Session.SetString("EncryptedPassword", encryptedPassword);
      return RedirectToAction("Admin_Home_Page", "Admin", new { sessionKey });
    }
    else
    {
      if (user == null)
      {
        ViewBag.LoginStatus = 0;
        return View(loginModel);
      }
      else
      {
        // Generate session key

        // Encrypt username and password using session key
        string encryptedUsername = EncryptionHelper.Encrypt(loginModel.LoginId, sessionKey);
        string encryptedPassword = EncryptionHelper.Encrypt(loginModel.Password, sessionKey);

        // Store session key and encrypted username and password in session
        HttpContext.Session.SetString("SessionKey", sessionKey);
        HttpContext.Session.SetString("EncryptedUsername", encryptedUsername);
        HttpContext.Session.SetString("EncryptedPassword", encryptedPassword);

        // Redirect to home page with session key in query string
        return RedirectToAction("Home_Page", "Home", new { sessionKey });
      }
    }

  }

  public IActionResult Home_Page(string sessionKey)
  {
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
      .Where(x => x.OrderStateId == 1)
      .Select(x => x)
      .OrderBy(x => x.OrderDate)
      .ToList();

    var orders = ordersListForUser
      .Skip(Math.Max(0, ordersListForUser.Count - 5))
      .OrderByDescending(x => x.OrderDate)
      .Select(x => new OrderViewModel
      {
        OrderNumber = ordersListForUser.IndexOf(x) + 1,
        State = x.OrderState.Name,
        OrderDate = x.OrderDate.ToString("d"),
        OrderAmount = x.OrderItems.Sum(y => y.Price)
      })
      .ToList();

    if (orders.Count == 0 || orders == null)
    {
      orders = new List<OrderViewModel>
      {
        new OrderViewModel { OrderNumber = -1, OrderDate = "", OrderAmount = -1, State = "" },
      };
    }
    double sumRestPriceToPay = _db.OrderItems
        .Where(x => x.Order.User.UserId == user.UserId).Where(x => x.Order.OrderStateId == 2).Sum(x => x.Price);

    var homeModel = new Home_PageModel
    {
      UserName = username,
      Orders = orders,
      SessionString = sessionKey,
      MoneyLeftToPay = sumRestPriceToPay
    };

    homeModel.SessionString = sessionKey;
    return View(homeModel);
  }

  public IActionResult Order_Detail(string sessionKey) => View(new Order_DetailModel { SessionString = sessionKey });

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

    // Encrypt username and password using session key
    string encryptedUsername = EncryptionHelper.Encrypt(user.UserName, sessionKey);
    string encryptedPassword = EncryptionHelper.Encrypt(loginModel.Password, sessionKey);

    // Store session key and encrypted username and password in session
    HttpContext.Session.SetString("SessionKey", sessionKey);
    HttpContext.Session.SetString("EncryptedUsername", encryptedUsername);
    HttpContext.Session.SetString("EncryptedPassword", encryptedPassword);

    // Redirect to home page with session key in query string
    return RedirectToAction("Home_Page", "Home", new { sessionKey });
  }

 
}
