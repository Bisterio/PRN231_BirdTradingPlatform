using BusinessObject.Common;
using BusinessObject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace BirdTradingPlatformStoreClient.Controllers
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
        public async Task<IActionResult> Index([FromQuery] int page, [FromQuery] int status, [FromQuery] string orderIdSearch)
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
                { "status", status.ToString() },
                { "orderIdSearch", orderIdSearch }
            };
            var dictFormUrlEncoded = new FormUrlEncodedContent(queryParameters);
            var queryString = await dictFormUrlEncoded.ReadAsStringAsync();

            // GET Request Order list
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.GetAsync(OrderApilUrl + $"/Store?{queryString}");
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
            HttpResponseMessage response = await client.GetAsync(OrderApilUrl + $"/Store/{id}");
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
            HttpResponseMessage response = await client.PutAsync(OrderApilUrl + $"/StoreCancel/{orderId}", content);
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ApproveOrder(long id)
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }

            // Approve Order
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.PutAsync(OrderApilUrl + $"/ApproveOrder/{id}", null);
            if ((int)response.StatusCode != 200)
            {
                TempData["ErrorMessage"] = "Cannot approve this order";
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

            TempData["SuccessMessage"] = "Approve order successfully";
            return RedirectToAction("Detail", new { id = id });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeliverOrder(long id)
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }

            // Approve Order
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.PutAsync(OrderApilUrl + $"/Deliver/{id}", null);
            if ((int)response.StatusCode != 200)
            {
                TempData["ErrorMessage"] = "Cannot deliver this order";
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

            TempData["SuccessMessage"] = "Confirm delivering successfully";
            return RedirectToAction("Detail", new { id = id });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CompleteOrder(long id)
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }

            // Approve Order
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.PutAsync(OrderApilUrl + $"/Complete/{id}", null);
            if ((int)response.StatusCode != 200)
            {
                TempData["ErrorMessage"] = "Cannot complete this order";
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

            TempData["SuccessMessage"] = "Confirm order complete successfully";
            return RedirectToAction("Detail", new { id = id });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CancelDecline(long id)
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }

            // Approve Order
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.PutAsync(OrderApilUrl + $"/CancelDeclined/{id}", null);
            if ((int)response.StatusCode != 200)
            {
                TempData["ErrorMessage"] = "Cannot decline cancel request";
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

            TempData["SuccessMessage"] = "Declined cancel request successfully";
            return RedirectToAction("Detail", new { id = id });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CancelApprove(long id)
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }

            // Approve Order
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.PutAsync(OrderApilUrl + $"/CancelApprove/{id}", null);
            if ((int)response.StatusCode != 200)
            {
                TempData["ErrorMessage"] = "Cannot approve cancel request";
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

            TempData["SuccessMessage"] = "Approved cancel request successfully";
            return RedirectToAction("Detail", new { id = id });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> RefundDecline(long id)
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }

            // Approve Order
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.PutAsync(OrderApilUrl + $"/RefundDecline/{id}", null);
            if ((int)response.StatusCode != 200)
            {
                TempData["ErrorMessage"] = "Cannot decline refund request";
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

            TempData["SuccessMessage"] = "Declined refund request successfully";
            return RedirectToAction("Detail", new { id = id });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> RefundAccept(long id)
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }

            // Approve Order
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.PutAsync(OrderApilUrl + $"/RefundAccept/{id}", null);
            if ((int)response.StatusCode != 200)
            {
                TempData["ErrorMessage"] = "Cannot approve refund request";
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

            TempData["SuccessMessage"] = "Approved refund request successfully";
            return RedirectToAction("Detail", new { id = id });
        }
    }
}
