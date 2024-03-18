using BusinessObject.Common;
using BusinessObject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace BirdTradingPlatformAdminClient.Controllers
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
        public async Task<IActionResult> Index([FromQuery] int page, [FromQuery] byte isReported)
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }

            // Get query parameters
            var queryParameters = new Dictionary<string, string>
            {
                { "page", page.ToString() },
                { "isReported", isReported.ToString() }
            };
            var dictFormUrlEncoded = new FormUrlEncodedContent(queryParameters);
            var queryString = await dictFormUrlEncoded.ReadAsStringAsync();

            // GET Request Order list
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.GetAsync(OrderApilUrl + $"/Admin?{queryString}");
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

            // GET Order Detail
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.GetAsync(OrderApilUrl + $"/Admin/{id}");
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
        [HttpGet]
        public async Task<IActionResult> RejectResolve(long id)
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }

            // Approve Order
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.PutAsync(OrderApilUrl + $"/ResolveReport/{id}", null);
            if ((int)response.StatusCode != 200)
            {
                TempData["ErrorMessage"] = "Cannot reject this refund report";
                return RedirectToAction("Detail", new { id = id });
            }
            string strData = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(strData);
            APIResult<string> result = data.ToObject<APIResult<string>>();
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Detail", new { id = id });
            }

            TempData["SuccessMessage"] = "Reject refund report successfully, this order will be set as Resolved";
            return RedirectToAction("Detail", new { id = id });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ApproveResolve(long id)
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }

            // Approve Order
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.PutAsync(OrderApilUrl + $"/ApproveRefundReport/{id}", null);
            if ((int)response.StatusCode != 200)
            {
                TempData["ErrorMessage"] = "Cannot resolve this refund report";
                return RedirectToAction("Detail", new { id = id });
            }
            string strData = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(strData);
            APIResult<string> result = data.ToObject<APIResult<string>>();
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Detail", new { id = id });
            }

            TempData["SuccessMessage"] = "Approve refund report successfullt, this order will be set as Resolved and will be refunded to the customer";
            return RedirectToAction("Detail", new { id = id });
        }
    }
}
