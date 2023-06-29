using Microsoft.AspNetCore.Mvc;

namespace BirdTradingPlatformClient.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
