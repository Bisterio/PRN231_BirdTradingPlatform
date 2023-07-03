using BusinessObject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Xml.Linq;

namespace BirdTradingPlatformClient.Controllers
{
    public class OrderController : Controller
    {
        private readonly HttpClient client = null;
        private string OrderApilUrl = "";

        public OrderController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            OrderApilUrl = "http://localhost:5208/api/Order";
        }

        [Authorize]
        public async Task<IActionResult> Index([FromQuery] int page, [FromQuery] int status)
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }

            // Get query parameters
            var queryParameters = new Dictionary<string, string>
            {{ "page", page.ToString() },{ "status", status.ToString() }};
            var dictFormUrlEncoded = new FormUrlEncodedContent(queryParameters);
            var queryString = await dictFormUrlEncoded.ReadAsStringAsync();

            // GET Request Order list
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.GetAsync(OrderApilUrl + $"/Customer?{queryString}");
            if ((int)response.StatusCode != 200)
            {
                return RedirectToAction("Logout", "Home");
            }
            string strData = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(strData);
            ClientOrderViewListDTO model = data.ToObject<ClientOrderViewListDTO>();

            return View(model);
        }

        [Authorize]
        public IActionResult Detail()
        {
            return View();
        }
    }
}
