using Microsoft.IdentityModel.Tokens;

namespace ProjectAspoeck.Controllers;

public class AdminController : Controller
{
    private readonly BreakfastDBContext _db = new();

    public IActionResult Admin_Home_Page()
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
            string encryptedPassword = HttpContext.Session.GetString("EncryptedPassword") ?? "";

            string username = EncryptionHelper.Decrypt(encryptedUsername, sessionKey);
            User user = _db.Users
                .Where(x => x.UserName == username)
                .FirstOrDefault() ?? new();

            var ordersList = _db.Orders.Include(x => x.OrderItems)
                .Where(x => x.OrderDate.Date == DateTime.Today.Date)
                .Take(5)
                .Select(x => new AdminOrderListDto
                {
                    OrderNumber = 1,
                    Date = x.OrderDate,
                    UserName = x.User.UserName,
                    Description = x.OrderItems.Select(x => $"{x.Quantity}x {x.Item.Name} ").First().ToString(),
                    State = x.OrderState.Name,
                    Price = Math.Round(x.OrderItems.Sum(x => x.Price), 2)
                }).ToList();

            /* if (ordersList.Count == 0 || ordersList == null)
             {
                 ordersList = new List<Admin_OrderListDTO>
               {
                 new Admin_OrderListDTO { OrderNumber= -1, Date = "", OrderItems = -1, State = "" },
               };
             }*/
            var adminhomeModel = new Admin_Home_PageModel
            {
                UserName = username,
                Orders = ordersList,
                SessionString = sessionKey,
            };

            adminhomeModel.SessionString = sessionKey;
            return View(adminhomeModel);
        }
    }

    [HttpGet]
    public void Admin_Export_List_OrdersFromDay()
    {
        // string file = "Downloads/liste.xlsx";
        string downloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/Downloads";
        string file = $"{downloadFolder}\\Tagesbestellungen_{DateTime.Now:dd_MM_yyyy}_Liste.csv";
        string euro = "\u20AC";
        string header = "\"Name\";\"Tagesbestellung\";\"Preis\"";
        using (StreamWriter writer = new StreamWriter(file))
        {
            writer.WriteLine($"{DateTime.Now.ToString("dd.MM.yyyy")}");
            writer.WriteLine();
            writer.WriteLine(header);
            foreach (Order orderFromTheDay in _db.Orders.Include(x => x.OrderItems).ThenInclude(x => x.Item).Include(x => x.User).Where(x => x.OrderDate.Date == DateTime.Now.Date).ToList())
            {
                writer.WriteLine($"{orderFromTheDay.User.FirstName} {orderFromTheDay.User.LastName};{orderFromTheDay.ToString()};{orderFromTheDay.OrderItems.Sum(x => x.Price):F2}");
            }
        }
    }

    [HttpGet]
    public void Admin_Export_List_PayDay()
    {
        // string file = "Downloads/liste.xlsx";
        string downloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/Downloads";
        string file = $"{downloadFolder}\\Zahltag_{DateTime.Now:dd_MM_yyyy}_Liste.csv";
        string euro = "\u20AC";
        string header = "\"Name\";\"Anzahl offener Bestellungen\";\"Preis (Summe)\"";
        using (StreamWriter writer = new StreamWriter(file))
        {
            writer.WriteLine($"{DateTime.Now.ToString("dd.MM.yyyy")}");
            writer.WriteLine();
            writer.WriteLine(header);
            foreach (var group in _db.Orders.Include(x => x.OrderItems).ThenInclude(x => x.Item).Include(x => x.User).Where(x => x.OrderStateId == 2).GroupBy(x => x.User))
            {
                var user = group.Key;
                var unpaidOrders = group.ToList();
                var totalPrice = unpaidOrders.Sum(x => x.OrderItems.Sum(item => item.Price));

                writer.WriteLine($"{user.FirstName} {user.LastName};{unpaidOrders.Count};{totalPrice:F2}");
            }
        }
    }

    [HttpPost]
    public IActionResult Admin_All_Orders()
    {
        string sessionKey = "notAuthorized";
        sessionKey = HttpContext.Session.GetString("SessionKey");
        if (sessionKey == "notAuthorized")
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            string encryptedUsername = HttpContext.Session.GetString("EncryptedUsername") ?? "";

            string username = EncryptionHelper.Decrypt(encryptedUsername, sessionKey);
            var user = _db.Users
                .Where(x => x.UserName == username)
                .FirstOrDefault() ?? new();

            var ordersListForUser = _db.Orders
                .Include(x => x.User)
                .Include(x => x.OrderState)
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.Item)
                .Select(x => x)
                .OrderBy(x => x.OrderId)
                .ToList();

            var orders = ordersListForUser
                .OrderByDescending(x => x.OrderDate)
                .Select(x => new AllOrdersViewModel
                {
                    OrderNumber = ordersListForUser.IndexOf(x) + 1,
                    OrderDate = x.OrderDate.ToString("d"),
                    OrderContent = x.ToString(),
                    OrderAmount = x.OrderItems.Sum(y => y.Price),
                    OrderState = x.OrderState.Name,
                    OrderPrice = x.OrderItems.Sum(x => x.Price)
                }).OrderBy(x => x.OrderNumber)
                .ToList();

            if (orders.Count == 0)
            {
                orders = new List<AllOrdersViewModel>
                {
                    new AllOrdersViewModel
                    {
                        OrderNumber = -1, OrderDate = "", OrderContent = "", OrderAmount = -1, OrderState = "",
                        OrderPrice = -1
                    },
                };
            }

            return View(new Admin_All_OrdersModel
            {
                Orders = orders,
                SessionString = sessionKey
            });
        }
    }

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
    public IActionResult AddUser(string firstname, string lastname, string username, string chipNr, string email)
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
    public IActionResult UpdateUser(int userId, string firstname, string lastname, string username, string chipNr, string email)
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

    [HttpPost]
    public IActionResult Admin_Edit_List()
    {
        string sessionKey = "notAuthorized";
        sessionKey = HttpContext.Session.GetString("SessionKey") ?? sessionKey;
        if (sessionKey == "notAuthorized")
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            return View(new Admin_Edit_ListModel
            {
                SessionKey = sessionKey,

            });
        }

        return View();
    }
}


