using Microsoft.AspNetCore.Mvc;

namespace KomfortWebApp.Controllers
{
    public class ServiceController : Controller
    {
        [HttpGet]
        public IActionResult Account()
        {
            return View();
        }
    }
}