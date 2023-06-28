using BirdTradingPlatformClient.Models;
using BusinessObject.Common;
using BusinessObject.DTOs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace BirdTradingPlatformClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient client = null;
        private string UserApilUrl = "";
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            UserApilUrl = "http://localhost:5208/api/User";
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                TempData["ReturnUrl"] = returnUrl;
            }
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Forbidden()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO request)
        {
            // Validate on client side
            if (!ModelState.IsValid)  return View(request);

            // Initiate ApiResult obj
            APIResult<string>? result;

            // Post LoginDTO request
            var jsonRequest = JsonConvert.SerializeObject(request);

            HttpResponseMessage response = await client.PostAsync(UserApilUrl + "/AuthenticateCustomer",
                new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
            
            // Get response
            // If login failed
            if (!response.IsSuccessStatusCode)
            {
                result = JsonConvert.DeserializeObject<APIErrorResult<string>>(await response.Content.ReadAsStringAsync());
                ViewBag.ErrorMessage = result?.Message;
                return View(request);
            }

            // If login success
            result = JsonConvert.DeserializeObject<APISuccessResult<string>>(await response.Content.ReadAsStringAsync());
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

            // Redirect
            object? returnUrl = string.Empty;
            TempData.TryGetValue("ReturnUrl", out returnUrl);
            string? returnUrlStr = returnUrl as string;
            if (!string.IsNullOrEmpty(returnUrlStr))
            {
                return Redirect(returnUrlStr);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
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
                return RedirectToAction("Index");
            }
            return Redirect(returnUrl);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}