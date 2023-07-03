using BusinessObject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace BirdTradingPlatformClient.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient client = null;
        private string UserApilUrl = "";

        public AccountController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            UserApilUrl = "http://localhost:5208/api/User";
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
