namespace ProjectAspoeck.Controllers;
public class LoginController : Controller
{
  private readonly BreakfastDBContext _db = new();
  private readonly ILogger<LoginController> _logger;
  public LoginController(ILogger<LoginController> logger) => _logger = logger;

  [HttpGet]
  public IActionResult Index() => View(new LoginModel());

  [HttpPost]
  public IActionResult Index(LoginModel loginModel)
  {
    User? user = _db.Users.Where(m => m.UserName == loginModel.LoginId && m.ChipNumber == loginModel.Password).FirstOrDefault();
    if (user == null)
    {
      ViewBag.LoginStatus = 0;
      return View(loginModel);
    }
    else
    {
      // Generate session key
      string sessionKey = Guid.NewGuid().ToString();

      string encryptedUsername = EncryptionHelper.Encrypt(loginModel.LoginId, sessionKey);
      string encryptedPassword = EncryptionHelper.Encrypt(loginModel.Password, sessionKey);

      // Store session key and encrypted username and password in session
      HttpContext.Session.SetString("SessionKey", sessionKey);
      HttpContext.Session.SetString("EncryptedUsername", encryptedUsername);
      HttpContext.Session.SetString("EncryptedPassword", encryptedPassword);

      // Redirect to home page with session key in query string
      return RedirectToAction("Index", "Home", new { sessionKey });
    }
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

    // Encrypt username and password using session key
    string encryptedUsername = EncryptionHelper.Encrypt(user.UserName, sessionKey);
    string encryptedPassword = EncryptionHelper.Encrypt(loginModel.Password, sessionKey);

    // Store session key and encrypted username and password in session
    HttpContext.Session.SetString("SessionKey", sessionKey);
    HttpContext.Session.SetString("EncryptedUsername", encryptedUsername);
    HttpContext.Session.SetString("EncryptedPassword", encryptedPassword);

    // Redirect to home page with session key in query string
    return RedirectToAction("Index", "Home", new { sessionKey });
  }
}
