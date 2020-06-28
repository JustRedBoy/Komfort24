using KomfortWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace KomfortWebApp.Controllers
{
    public class RatesController : Controller
    {
        private readonly GoogleSheets _googleSheets;

        public RatesController(GoogleSheets googleSheets)
        {
            _googleSheets = googleSheets;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _googleSheets.GetRatesAsync());
        }
    }
}