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
            List<UserDTO> _users = _db.Users.Select(x=> new UserDTO {LastName = x.LastName, FirstName = x.FirstName}).ToList();
            Console.WriteLine(_users);
            /*if (_loginModel.LoginId == null || _loginModel.ChipNr == null)
            {
                _loginModel.LoginId = "asdf";
                _loginModel.ChipNr = "1234";

            }*/
            
            User? status = _db.Users.Where(m => m.UserName == _loginModel.LoginId && m.ChipNumber == _loginModel.Pasword).FirstOrDefault();
            if (status == null)
            {
                ViewBag.LoginStatus = 0;
            }
            else
            {
                return RedirectToAction("SuccessPage", "Home");
            }
            return View(_loginModel);
        }
        public IActionResult SuccessPage()
        {
            return View();
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