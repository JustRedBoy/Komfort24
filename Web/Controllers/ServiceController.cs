using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
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