using BusinessObject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BirdTradingPlatformClient.Controllers
{
    public class StoreController : Controller
    {
        private readonly HttpClient client = null;
        private string StoreApilUrl = "";

        public StoreController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            StoreApilUrl = "http://localhost:5208/api/Store/Public";
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            // GET Request Store list
            HttpResponseMessage response = await client.GetAsync(StoreApilUrl);
            string strData = await response.Content.ReadAsStringAsync();
            List<ClientStoreDetailViewDTO>? model = JsonSerializer.Deserialize<List<ClientStoreDetailViewDTO>>(strData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Detail(long id)
        {
            // GET Request Store Detail
            HttpResponseMessage response = await client.GetAsync(StoreApilUrl + $"/{id}");
            if ((int)response.StatusCode != 200)
            {
                return RedirectToAction("Index");
            }
            string strData = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(strData);
            ClientStoreDetailViewDTO model = data.ToObject<ClientStoreDetailViewDTO>();

            return View(model);
        }
    }
}
