using BusinessObject.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace BirdTradingPlatformClient.Controllers
{
    public class CartController : Controller
    {
        private readonly HttpClient client = null;
        private string ProductApilUrl = "";

        public CartController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApilUrl = "http://localhost:5208/api/Product";
        }

        public IActionResult Index()
        {
            return View();
        }

        // Ham nay nho authorize
        [HttpPost]
        public IActionResult Index([FromBody] CartAddressDTO request)
        {
            return View();
        }

        public IActionResult Checkout()
        {
            return View();
        }
    }
}
