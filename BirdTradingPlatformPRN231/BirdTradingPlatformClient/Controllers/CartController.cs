using BusinessObject.Common;
using BusinessObject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace BirdTradingPlatformClient.Controllers
{
    public class CartController : Controller
    {
        private readonly HttpClient client = null;
        private string ProductApilUrl = "";
        private string OrderApilUrl = "";

        public CartController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApilUrl = "http://localhost:5208/api/Product";
            OrderApilUrl = "http://localhost:5208/api/Order";
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GetShippingCost([FromBody] CartAddressDTO request)
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }

            // Post Request Check shipping cost
            string postJson = JsonConvert.SerializeObject(request,
               new JsonSerializerSettings
               {
                   DateTimeZoneHandling = DateTimeZoneHandling.Local
               });
            StringContent content = new StringContent(postJson, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.PostAsync(ProductApilUrl + "/CalculateShip", content);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }
            string strData = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(strData);
            APIResult<CheckoutViewDTO> result = data.ToObject<APIResult<CheckoutViewDTO>>();
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDTO request)
        {
            // Check modelstate
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Post Request Check shipping cost
            string postJson = JsonConvert.SerializeObject(request,
               new JsonSerializerSettings
               {
                   DateTimeZoneHandling = DateTimeZoneHandling.Local
               });
            StringContent content = new StringContent(postJson, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.PostAsync(OrderApilUrl, content);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Unauthorized();
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest(response);
            }
            string strData = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(strData);
            APIResult<string> result = data.ToObject<APIResult<string>>();
            return Ok(result);
        }

        [Authorize]
        public IActionResult Checkout()
        {
            return View();
        }
    }
}
