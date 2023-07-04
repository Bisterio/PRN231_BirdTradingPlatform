using BirdTradingPlatformStoreClient.Models;
using BusinessObject.Common;
using BusinessObject.DTOs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Firebase.Auth;
using Firebase.Storage;

namespace BirdTradingPlatformStoreClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient client = null;
        private string UserApilUrl = "";
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public HomeController(IConfiguration configuration, IWebHostEnvironment env)
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            UserApilUrl = "http://localhost:5208/api/User";
            _configuration = configuration;
            _env = env;
        }

        // Configure firebase
        private static string ApiKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("firebaseConfig")["apiKey"];
        private static string Bucket = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("firebaseConfig")["storageBucket"];
        private static string AuthEmail = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("firebaseConfig")["authEmail"];
        private static string AuthPassword = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("firebaseConfig")["authPassword"];

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }

            return View(new LoginDTO());
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }
            return View(new StoreCreateFormModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(StoreCreateFormModel model)
        {
            // Validate on client side
            if (!ModelState.IsValid) return View(model);

            // Initiate ApiResult obj
            APIResult<bool>? result;

            // Validate Confirm Password
            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ViewBag.ErrorMessage = "Confirm Password must match Password";
                return View(model);
            }

            // Upload Image
            // Get Img file
            var fileUploadLogo = model.UploadLogoImage;
            var fileUploadCover = model.UploadCoverImage;

            // Upload Logo img
            if (fileUploadLogo != null && fileUploadLogo.Length > 0)
            {
                model.NewStoreLogoImage = await UploadImage(fileUploadLogo);
            }

            // Upload Cover img
            if (fileUploadCover != null && fileUploadCover.Length > 0)
            {
                model.NewStoreCoverImage = await UploadImage(fileUploadCover);
            }

            RegisterStoreDTO dto = new RegisterStoreDTO()
            {
                Email = model.Email,
                Name = model.Name,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,
                Phone = model.Phone,
                NewStoreName = model.NewStoreName,
                NewStoreAddress = model.NewStoreAddress,
                NewStoreDescription = model.NewStoreDescription,
                NewStoreLogoImage = model.NewStoreLogoImage,
                NewStoreCoverImage = model.NewStoreCoverImage
            };

            // Post RegisterStoreDTO request
            var jsonRequest = JsonConvert.SerializeObject(dto);

            HttpResponseMessage response = await client.PostAsync(UserApilUrl + "/RegisterStore",
                new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

            // Get response
            // If register failed
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Can't register a new account";
                return View(model);
            }

            result = JsonConvert.DeserializeObject<APIResult<bool>>(await response.Content.ReadAsStringAsync());
            if (!result.IsSuccess)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(model);
            }

            ViewBag.SuccessMessage = "Create a new store account successfully";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO request)
        {
            // Validate on client side
            if (!ModelState.IsValid) return View(request);

            // Initiate ApiResult obj
            APIResult<string>? result;

            // Post LoginDTO request
            var jsonRequest = JsonConvert.SerializeObject(request);

            HttpResponseMessage response = await client.PostAsync(UserApilUrl + "/AuthenticateStore",
                new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

            // Get response
            // If login failed
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Email or Password is incorrect";
                return View(request);
            }

            result = JsonConvert.DeserializeObject<APIResult<string>>(await response.Content.ReadAsStringAsync());
            if (!result.IsSuccess)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(request);
            }

            // If login success
            var userPrincipal = this.ValidateToken(result.ResultObj);
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = false
            };

            HttpContext.Session.SetString("Token", result.ResultObj);
            await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        userPrincipal,
                        authProperties);

            return RedirectToAction("Index", "Home");
        }

        private ClaimsPrincipal ValidateToken(string jwtToken)
        {
            IdentityModelEventSource.ShowPII = true;

            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();

            validationParameters.ValidateLifetime = true;

            validationParameters.ValidAudience = _configuration["JWT:Issuer"];
            validationParameters.ValidIssuer = _configuration["JWT:Issuer"];
            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);

            return principal;
        }

        [Authorize]
        public IActionResult Logout(string returnUrl)
        {
            // Remove Identity
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("Token");

            // Back to home
            if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("Login");
            }
            return Redirect(returnUrl);
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}