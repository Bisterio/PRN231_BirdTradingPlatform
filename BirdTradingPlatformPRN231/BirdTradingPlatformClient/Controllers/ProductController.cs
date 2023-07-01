using BusinessObject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace BirdTradingPlatformClient.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient client = null;
        private string ProductApilUrl = "";

        public ProductController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApilUrl = "http://localhost:5208/api/Product";
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index([FromQuery] int page, [FromQuery] string? name, [FromQuery] long category,
            [FromQuery] long pmin, [FromQuery] long pmax, [FromQuery] int order)
        {
            if (pmax == 0) pmax = 10000000;
            // Get query parameters
            var queryParameters = new Dictionary<string, string>
                {
                    { "page", page.ToString() },
                    { "name", name },
                    { "category", category.ToString() },
                    { "pmin", pmin.ToString() },
                    { "pmax", pmax.ToString() },
                    { "order", order.ToString() },
                };
            var dictFormUrlEncoded = new FormUrlEncodedContent(queryParameters);
            var queryString = await dictFormUrlEncoded.ReadAsStringAsync();

            // GET Request Product list
            HttpResponseMessage response = await client.GetAsync(ProductApilUrl + $"/Public?{queryString}");
            string strData = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(strData);
            ClientProductViewListDTO model = data.ToObject<ClientProductViewListDTO>();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(long id)
        {
            // GET Request Product Detail
            HttpResponseMessage response = await client.GetAsync(ProductApilUrl + $"/Public/{id}");
            if ((int)response.StatusCode != 200)
            {
                return RedirectToAction("Index");
            }
            string strData = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(strData);
            ProductViewDTO model = data.ToObject<ProductViewDTO>();

            return View(model);
        }
    }
}
