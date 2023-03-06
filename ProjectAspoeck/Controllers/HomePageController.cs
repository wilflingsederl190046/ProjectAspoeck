using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProjectAspoeck.Controllers
{
    public class HomePageController : Controller
    {
        // GET: HomePageController
        public ActionResult Home_Page()
        {
            ViewBag.Message = "Hello, world!";
            return View();
        }

        // GET: HomePageController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HomePageController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HomePageController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomePageController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HomePageController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomePageController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HomePageController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
