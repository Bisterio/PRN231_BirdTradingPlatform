using Microsoft.AspNetCore.Mvc;

namespace BirdTradingPlatformClient.Controllers
{
    public class InvoiceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Detail()
        {
            return View();
        }
    }
}
