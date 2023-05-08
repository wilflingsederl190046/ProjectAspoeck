using Microsoft.IdentityModel.Tokens;

namespace ProjectAspoeck.Controllers.Admin;

public class ManageUsersController : Controller
{
    private readonly BreakfastDBContext _db = new();

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

            return View(new AdminManageUsersModel
            {
                SessionString = sessionKey,
                Users = allUsers
            });
        }
    }

    [HttpPost]
    public IActionResult DeleteUser(int userId)
    {
        try
        {
            var user = _db.Users.FirstOrDefault(x => x.UserId == userId);
            if (user == null) return Json(new { success = false, message = "Benutzer nicht gefunden." });
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
    public IActionResult ChangePassword(int userId, string newPassword)
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
        string email)
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
    public IActionResult ChangeActive(int userId, bool isActive)
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
                message = ex.Message
            });
        }
    }
}