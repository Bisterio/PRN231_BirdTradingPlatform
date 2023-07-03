using BusinessObject.DTOs;
using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Xml.Linq;

namespace BirdTradingPlatformClient.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly HttpClient client = null;
        private string InvoiceApilUrl = "";

        public InvoiceController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            InvoiceApilUrl = "http://localhost:5208/api/Invoice/Customer";
        }

        [Authorize]
        public async Task<IActionResult> Index([FromQuery] int page)
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }

            // Get query parameters
            var queryParameters = new Dictionary<string, string>
            {{ "page", page.ToString() }};
            var dictFormUrlEncoded = new FormUrlEncodedContent(queryParameters);
            var queryString = await dictFormUrlEncoded.ReadAsStringAsync();

            // GET Request Store Detail
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.GetAsync(InvoiceApilUrl + $"?{queryString}");
            if ((int)response.StatusCode != 200)
            {
                return RedirectToAction("Logout","Home");
            }
            string strData = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(strData);
            ClientInvoiceViewListDTO model = data.ToObject<ClientInvoiceViewListDTO>();

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
            HttpResponseMessage response = await client.GetAsync(InvoiceApilUrl + $"/{id}");
            if ((int)response.StatusCode != 200)
            {
                return RedirectToAction("Index");
            }
            string strData = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(strData);
            InvoiceViewDTO model = data.ToObject<InvoiceViewDTO>();

            return View(model);
        }
    }
}
