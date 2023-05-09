namespace ProjectAspoeck.Controllers;

public class ConfirmOrderController: Controller
{
    private readonly BreakfastDBContext _db = new();

    [HttpPost]
    public IActionResult ConfirmOrder()
    {
        string sessionKey = "notAuthorized";
        sessionKey = HttpContext.Session.GetString("SessionKey")?? sessionKey;
        if (sessionKey == "notAuthorized")
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            var secretUserName = HttpContext.Session.GetString("EncryptedUsername") ?? "Anonymous";
            string userName = EncryptionHelper.Decrypt(secretUserName, sessionKey);
            var user = _db.Users.FirstOrDefault(u => u.UserName == userName);
            int userId = user.UserId;
            var userTodaysOrder = _db.Orders.Include(x => x.User).Include(x=> x.OrderItems).Include(x=>x.OrderState)
                .Where(x => x.UserId == user.UserId && x.OrderDate.Date == DateTime.Today).FirstOrDefault();
            userTodaysOrder.OrderStateId= 2;
            _db.SaveChanges();
            return RedirectToAction("LoginWithChip", "Home");
        }

        
    }

    [HttpPost]
    public IActionResult CancelOrder()
    {
        string sessionKey = "notAuthorized";
        sessionKey = HttpContext.Session.GetString("SessionKey") ?? sessionKey;
        if (sessionKey == "notAuthorized")
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            var secretUserName = HttpContext.Session.GetString("EncryptedUsername") ?? "Anonymous";
            string userName = EncryptionHelper.Decrypt(secretUserName, sessionKey);
            var user = _db.Users.FirstOrDefault(u => u.UserName == userName);
            int userId = user.UserId;
            var userTodaysOrder = _db.Orders.Include(x => x.User).Include(x => x.OrderItems).Include(x => x.OrderState)
                .Where(x => x.UserId == user.UserId && x.OrderDate.Date == DateTime.Today).FirstOrDefault();
            userTodaysOrder.OrderStateId = 4;
            _db.SaveChanges();
            return RedirectToAction("LoginWithChip", "Home");
        }
    }
}