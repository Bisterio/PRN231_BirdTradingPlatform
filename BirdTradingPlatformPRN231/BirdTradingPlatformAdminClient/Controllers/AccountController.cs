using BusinessObject.Common;
using BusinessObject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace BirdTradingPlatformAdminClient.Controllers
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

        [Authorize]
        public async Task<IActionResult> Index([FromQuery] int page, [FromQuery] string roleSearch)
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
                { "roleSearch", roleSearch }
            };
            var dictFormUrlEncoded = new FormUrlEncodedContent(queryParameters);
            var queryString = await dictFormUrlEncoded.ReadAsStringAsync();

            // GET Request Order list
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.GetAsync(UserApilUrl + $"/All?{queryString}");
            if ((int)response.StatusCode != 200)
            {
                return RedirectToAction("Logout", "Home");
            }
            string strData = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(strData);
            ClientUserViewListDTO model = data.ToObject<ClientUserViewListDTO>();

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
            HttpResponseMessage response = await client.GetAsync(UserApilUrl + $"/Detail/{id}");
            if ((int)response.StatusCode != 200)
            {
                return RedirectToAction("Index");
            }
            string strData = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(strData);
            APIResult<UserDetailViewDTO> model = data.ToObject<APIResult<UserDetailViewDTO>>();

            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }

            return View(model.ResultObj);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ChangeStatus(long id)
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }

            // Approve Order
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.DeleteAsync(UserApilUrl + $"/{id}");
            if ((int)response.StatusCode != 200)
            {
                TempData["ErrorMessage"] = "Cannot change status of this User";
                return RedirectToAction("Detail", new { id = id });
            }
            string strData = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(strData);
            APIResult<bool> result = data.ToObject<APIResult<bool>>();
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Detail", new { id = id });
            }

            TempData["SuccessMessage"] = "Change status successfully";
            return RedirectToAction("Detail", new { id = id });
        }
    }
}
