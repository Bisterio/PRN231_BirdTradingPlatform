using BusinessObject.Common;
using BusinessObject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
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
        public async Task<IActionResult> Detail(long id)
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }

            // GET Invoice Detail
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.GetAsync(OrderApilUrl + $"/Customer/{id}");
            if ((int)response.StatusCode != 200)
            {
                return RedirectToAction("Index");
            }
            string strData = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(strData);
            OrderViewDTO model = data.ToObject<OrderViewDTO>();

            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Cancel([FromForm] long orderId, [FromForm] string cancelReason)
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }

            // Cancel Order
            string jsonString = JsonConvert.SerializeObject(cancelReason);
            StringContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.PutAsync(OrderApilUrl + $"/CustomerCancel/{orderId}", content);
            if ((int)response.StatusCode != 200)
            {
                TempData["ErrorMessage"] = "Cannot cancel this order";
                return RedirectToAction("Detail", new { id = orderId });
            }
            string strData = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(strData);
            APIResult<string> result = data.ToObject<APIResult<string>>();
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Detail", new { id = orderId });
            }

            TempData["SuccessMessage"] = "Cancel order successfully";
            return RedirectToAction("Detail", new { id = orderId });
        }
    }
}
