using Microsoft.AspNetCore.Mvc;

namespace BirdTradingPlatformClient.Controllers
{
    public class OrderController : Controller
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
