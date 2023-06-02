using Microsoft.IdentityModel.Tokens;
using ProjectAspoeck.Models.Admin;

namespace ProjectAspoeck.Controllers.Admin;

public class ManageUsersController : Controller
{
    private readonly BreakfastDBContext _db;
    public ManageUsersController(BreakfastDBContext db) => _db = db;

    public IActionResult Admin_Manage_Users()
    {
        string sessionKey = "notAuthorized";
        sessionKey = HttpContext.Session.GetString("SessionKey") ?? sessionKey;
        if (sessionKey == "notAuthorized")
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            
            var allUsers = _db.Users
                .Select(x => new AdminUserDto
                {
                    UserId = x.UserId,
                    UserName = x.UserName,
                    FirstName = x.FirstName,
                    ChipNumber = x.ChipNumber,
                    Active = x.Active,
                    CreatedDate = x.CreatedDate,
                    Email = x.Email,
                    LastName = x.LastName,
                    UserPassword = x.UserPassword
                })
                .ToList();

            return View(new ManageUsersModelAdmin
            {
                SessionString = sessionKey,
                Users = allUsers,
                SearchString = HttpContext.Session.GetString("SearchString") ?? ""
            });
        }
    }

    [HttpPost]
    public IActionResult DeleteUser(int userId, string searchString)
    {
        try
        {
            
            var user = _db.Users.FirstOrDefault(x => x.UserId == userId);
            if (user == null) return Json(new { success = false, message = "Benutzer nicht gefunden." });
            HttpContext.Session.SetString("SearchString", searchString);
            // Delete Settings
            _db.Settings.RemoveRange(_db.Settings.Where(x => x.User.UserId == userId));
            _db.SaveChanges();
            
            var allOrders = _db.Orders.Where(x => x.User.UserId == userId).ToList();
            // Delete all OrderItems
            allOrders.ForEach(x=> x.OrderItems.ToList().ForEach(y=> _db.OrderItems.Remove(y)));
            // Delete all Orders
            allOrders.ForEach(x => _db.Orders.Remove(x));
            _db.SaveChanges();
            
            _db.Users.Remove(user);
            _db.SaveChanges();
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult ChangePassword(int userId, string newPassword, string searchString)
    {
        try
        {
            
            var user = _db.Users.Find(userId);
            if (user == null || newPassword == null)
            {
                return Json(new
                {
                    success = false,
                    message = "User not found"
                });
            }
            HttpContext.Session.SetString("SearchString", searchString);

            user.SetPassword(newPassword);
            _db.SaveChanges();

            return Json(new
            {
                success = true
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    [HttpPost]
    public IActionResult AddUser(string firstname, string lastname, string username, string chipNr, string email, string password)
    {
        try
        {
            var user = new User
            {
                FirstName = firstname,
                LastName = lastname,
                UserName = username,
                ChipNumber = chipNr
            };
            user.SetPassword(password);
            if (!email.IsNullOrEmpty()) user.Email = email;
            _db.Users.Add(user);
            _db.SaveChanges();

            _db.Settings.Add(new Setting { User = user});
            _db.SaveChanges();
            
            return Json(new
            {
                success = true
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    [HttpPost]
    public IActionResult UpdateUser(int userId, string firstname, string lastname, string username, string chipNr,
        string email, string searchString)
    {
        try
        {
            var user = _db.Users.SingleOrDefault(x => x.UserId == userId);
            if (user == null)
            {
                return Json(new
                {
                    success = false,
                    message = "User not found"
                });
            }
            HttpContext.Session.SetString("SearchString", searchString);

            user.FirstName = firstname;
            user.LastName = lastname;
            user.UserName = username;
            user.ChipNumber = chipNr;
            user.Email = email;
            _db.SaveChanges();

            return Json(new
            {
                success = true
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    [HttpPost]
    public IActionResult GetUser(int userId)
    {
        try
        {
            var selectedUser = _db.Users.SingleOrDefault(x => x.UserId == userId);

            if (selectedUser == null)
            {
                return Json(new
                {
                    success = false,
                    message = "User not found"
                });
            }

            return Json(new
            {
                success = true,
                userFirstname = selectedUser.FirstName,
                userLastname = selectedUser.LastName,
                userUsername = selectedUser.UserName,
                userChipNr = selectedUser.ChipNumber,
                userEmail = selectedUser.Email
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message 
            });
        }
    }

    [HttpPost]
    public IActionResult ChangeActive(int userId, bool isActive, string searchString)
    {
        try
        {
            var selectedUser = _db.Users.SingleOrDefault(x => x.UserId == userId);

            if (selectedUser == null)
            {
                return Json(new
                {
                    success = false,
                    message = "User not found"
                });
            }

            if (searchString == null)
            {
                searchString="";
            }

            HttpContext.Session.SetString("SearchString", searchString);
            selectedUser.Active = isActive;
            _db.SaveChanges();

            return Json(new
            {
                success = true
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message +" changes to active state failed"
            });
        }
    }
}