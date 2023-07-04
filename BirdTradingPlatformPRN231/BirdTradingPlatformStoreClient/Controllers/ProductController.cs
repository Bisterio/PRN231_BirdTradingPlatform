using BirdTradingPlatformStoreClient.Models;
using BusinessObject.Common;
using BusinessObject.DTOs;
using BusinessObject.Models;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BirdTradingPlatformStoreClient.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient client = null;
        private string ProductApilUrl = "";
        private string CategoryApilUrl = "";
        private readonly IWebHostEnvironment _env;

        public ProductController(IWebHostEnvironment env)
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApilUrl = "http://localhost:5208/api/Product";
            CategoryApilUrl = "http://localhost:5208/api/Category";
            _env = env;
        }

        // Configure firebase
        private static string ApiKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("firebaseConfig")["apiKey"];
        private static string Bucket = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("firebaseConfig")["storageBucket"];
        private static string AuthEmail = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("firebaseConfig")["authEmail"];
        private static string AuthPassword = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("firebaseConfig")["authPassword"];

        [Authorize]
        public async Task<IActionResult> Index([FromQuery] int page, [FromQuery] string? name)
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
                    { "name", name }
                };
            var dictFormUrlEncoded = new FormUrlEncodedContent(queryParameters);
            var queryString = await dictFormUrlEncoded.ReadAsStringAsync();

            // GET Request Product list
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.GetAsync(ProductApilUrl + $"/CurrentStore?{queryString}");
            string strData = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(strData);
            ClientProductViewListDTO model = data.ToObject<ClientProductViewListDTO>();

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Detail(long id)
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }

            // GET Request Product Detail
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.GetAsync(ProductApilUrl + $"/CurrentStore/{id}");
            if ((int)response.StatusCode != 200)
            {
                return RedirectToAction("Index");
            }
            string strData = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(strData);
            ProductCreateDTO dto = data.ToObject<ProductCreateDTO>();
            ProductCreateFormModel model = new ProductCreateFormModel()
            {
                Categories = dto.Categories,
                CategoryId = dto.CategoryId,
                Description = dto.Description,
                Image = dto.Image,
                Name = dto.Name,
                ProductId = dto.ProductId,
                Stock = dto.Stock,
                UnitPrice = dto.UnitPrice
            };

            if(TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Detail(ProductCreateFormModel model)
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }

            // GET Category list
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.GetAsync(CategoryApilUrl);
            string strData = await response.Content.ReadAsStringAsync();
            List<Category>? items = System.Text.Json.JsonSerializer.Deserialize<List<Category>>(strData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            model.Categories = items;

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Validation error!";
                return View(model);
            }

            // Upload Image
            // Get Img file
            var fileUpload = model.UploadImage;

            if (fileUpload != null && fileUpload.Length > 0)
            {
                model.Image = await UploadImage(fileUpload);
            }

            ProductCreateDTO dto = new ProductCreateDTO
            {
                CategoryId = model.CategoryId,
                UnitPrice = model.UnitPrice,
                Description = model.Description,
                Image = model.Image,
                Name = model.Name,
                Stock = model.Stock
            };

            // Update Product
            string jsonString = JsonConvert.SerializeObject(dto);
            StringContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage responsePut = await client.PutAsync(ProductApilUrl + $"/{model.ProductId}", content);
            if ((int)response.StatusCode != 200)
            {
                ViewBag.ErrorMessage = "Cannot update this product";
                return View(model);
            }
            string strDataPut = await responsePut.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(strDataPut);
            APIResult<long> result = data.ToObject<APIResult<long>>();
            if (!result.IsSuccess)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(model);
            }

            ViewBag.SuccessMessage = "Update this product successfully";
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }

            // GET Category list
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.GetAsync(CategoryApilUrl);
            string strData = await response.Content.ReadAsStringAsync();
            List<Category>? items = System.Text.Json.JsonSerializer.Deserialize<List<Category>>(strData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(new ProductCreateFormModel
            {
                Categories= items
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateFormModel model)
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }

            // GET Category list
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.GetAsync(CategoryApilUrl);
            string strData = await response.Content.ReadAsStringAsync();
            List<Category>? items = System.Text.Json.JsonSerializer.Deserialize<List<Category>>(strData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            model.Categories= items;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Upload Image
            // Get Img file
            var fileUpload = model.UploadImage;

            if (fileUpload != null && fileUpload.Length > 0)
            {
                model.Image = await UploadImage(fileUpload);
            }

            ProductCreateDTO dto = new ProductCreateDTO
            {
                CategoryId = model.CategoryId,
                UnitPrice = model.UnitPrice,
                Description = model.Description,
                Image = model.Image,
                Name = model.Name,
                Stock = model.Stock
            };

            // Post Product
            string jsonString = JsonConvert.SerializeObject(dto);
            StringContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage responsePost = await client.PostAsync(ProductApilUrl, content);
            if ((int)response.StatusCode != 200)
            {
                ViewBag.ErrorMessage = "Cannot create product";
                return View(model);
            }
            string strDataPost = await responsePost.Content.ReadAsStringAsync();
            dynamic dataPost = JObject.Parse(strDataPost);
            APIResult<long> resultPost = dataPost.ToObject<APIResult<long>>();
            if (!resultPost.IsSuccess)
            {
                ViewBag.ErrorMessage = "Cannot create product";
                return View(model);
            }

            TempData["SuccessMessage"] = "Create product successfully";
            return RedirectToAction("Detail", new { id = resultPost.ResultObj });
        }

        public async Task<string> UploadImage(IFormFile file)
        {
            FileStream fs;
            string fileExtension = Path.GetExtension(file.FileName).Substring(1);
            string fileName = $"{Path.GetRandomFileName()}.{fileExtension}";
            // Upload file to firebase
            string folderName = "upload-image";
            string path = Path.Combine(_env.WebRootPath, $"{folderName}");
            using (fs = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }
            fs = new FileStream(Path.Combine(path, fileName), FileMode.Open);
            // Firebase uploading
            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

            // Cancellation Token
            var upload = new FirebaseStorage
            (
                Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                }
            )
            .Child(fileName)
            .PutAsync(fs);

            var downloadUrl = await upload;

            return downloadUrl;
        }
    }
}
