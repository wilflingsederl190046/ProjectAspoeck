using BreakfastDBLib;
using Microsoft.AspNetCore.Mvc;
using ProjectAspoeck.Models;
using System.Diagnostics;
using DTOLibary;

namespace ProjectAspoeck.Controllers
{
    public class HomeController : Controller
    {
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
            BreakfastContext _db = new BreakfastContext();
            List<UserDTO> _users = _db.Users.Select(x=> new UserDTO {UserId= x.UserId ,LastName = x.LastName, FirstName = x.FirstName}).ToList();
            Console.WriteLine(_users);
            /*if (_loginModel.LoginId == null || _loginModel.ChipNr == null)
            {
                _loginModel.LoginId = "asdf";
                _loginModel.ChipNr = "1234";

            }*/
            
            User? user = _db.Users.Where(m => m.UserName == _loginModel.LoginId && m.ChipNumber == _loginModel.Pasword).FirstOrDefault();
            if (user == null)
            {
                
                ViewBag.LoginStatus = 0;
            }
            else
            {
                _loginModel.UserId = user.UserId;
                return RedirectToAction("Home_Page", "Home");
            }
            return View(_loginModel);
        }
        public IActionResult Home_Page(Home_PageModel _homeModel, LoginModel loginModel)
        {
            _homeModel.UserId = loginModel.UserId;
            return View(_homeModel);
        }



        public IActionResult Privacy()
        {
            return View();
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}