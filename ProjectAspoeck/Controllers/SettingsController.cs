namespace ProjectAspoeck.Controllers;
public class SettingsController : Controller
{
  private readonly BreakfastDBContext _db;
  public SettingsController(BreakfastDBContext db) => _db = db;
  
  public IActionResult Settings()
  {
    var settingsModel  = new SettingsModel();
    string sessionKey = "notAuthorized";
    sessionKey = HttpContext.Session.GetString("SessionKey") ?? sessionKey;
    if (sessionKey == "notAuthorized")
    {
      return RedirectToAction("Index", "Home");
    }
    else
    {
      string encryptedUsername = HttpContext.Session.GetString("EncryptedUsername") ?? "";
      string username = EncryptionHelper.Decrypt(encryptedUsername, sessionKey);
      User user = _db.Users
        .Where(x => x.UserName == username)
        .FirstOrDefault() ?? new();
      Setting settings = _db.Settings
        .Where(x => x.UserId == user.UserId)
        .FirstOrDefault() ?? new();
      settingsModel  = new SettingsModel
      {
        Email = user.Email,
        RememberToOrder = settings.NotificationOrderDeadline,
        RememberToPay = settings.NotificationPaymentDeadline,
        MinutesBefore = settings.MinutesBefore,
        DaysBefore = settings.DaysBefore,
        SessionString = sessionKey
      };
    }
    return View(settingsModel);
  }
  [HttpPost]
  public IActionResult SaveSettings(string email, bool rto, bool rtp, int minBef, int daysBef)
  {
    string sessionKey = "notAuthorized";
    sessionKey = HttpContext.Session.GetString("SessionKey") ?? sessionKey;
    if (sessionKey == "notAuthorized")
    {
     
      return RedirectToAction("Index", "Home");
    }
    else
    {
      string encryptedUsername = HttpContext.Session.GetString("EncryptedUsername") ?? "";
      string username = EncryptionHelper.Decrypt(encryptedUsername, HttpContext.Session.GetString("SessionKey"));
      
          User user = _db.Users
            .Where(x => x.UserName == username)
            .FirstOrDefault() ?? new();
      
          Setting settings = _db.Settings
            .Where(x => x.UserId == user.UserId)
            .FirstOrDefault() ?? new();
      
          user.Email = email;
          settings.NotificationOrderDeadline = rto;
          settings.NotificationPaymentDeadline = rtp;
          settings.DaysBefore = daysBef;
          settings.MinutesBefore = minBef;
          _db.SaveChanges();
          return Ok(email);
    }
    
  }
}
