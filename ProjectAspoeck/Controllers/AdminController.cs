﻿namespace ProjectAspoeck.Controllers;

public class AdminController : Controller
{
    private readonly BreakfastDBContext _db = new();

    public IActionResult Admin_Home_Page()
    {
        string sessionKey = "notAuthorized";
        sessionKey = HttpContext.Session.GetString("SessionKey");
        if (sessionKey == "notAuthorized")
        {
            RedirectToAction("Index", "Home");
        }
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

    [HttpGet]
    public void Admin_Export_List()
    {
       // string file = "Downloads/liste.xlsx";
        string file = "C:\\Users\\Test\\Downloads\\liste.xls";

        using (StreamWriter writer = new StreamWriter(file))
        {
            writer.WriteLine($"{DateTime.Now.Date}");
            writer.WriteLine();
            foreach(Order orderFromTheDay in _db.Orders.Include(x => x.OrderItems).Include(x => x.User).ToList())
            {
                writer.WriteLine($"{orderFromTheDay.User.LastName};{orderFromTheDay.ToString()};{orderFromTheDay.OrderItems.Sum(x => x.Price)}");
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
    
    public IActionResult Admin_Manage_Users()
    {
        string sessionKey = "notAuthorized";
        sessionKey = HttpContext.Session.GetString("SessionKey") ?? sessionKey;
        if (sessionKey == "notAuthorized")
        {
            return RedirectToAction("Index", "Home");
        }

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
                return Json(new {
                    success = false,
                    message = "User not found"
                });
            }
            
            user.SetPassword(newPassword);
            _db.SaveChanges();

            return Json(new {
                success = true
            });
        }
        catch (Exception ex)
        {
            return Json(new {
                success = false,
                message = ex.Message
            });
        }
    }
}
