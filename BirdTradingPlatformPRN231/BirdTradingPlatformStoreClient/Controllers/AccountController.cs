using BusinessObject.Common;
using BusinessObject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Text;
using Firebase.Auth;
using Firebase.Storage;

namespace BirdTradingPlatformStoreClient.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient client = null;
        private string UserApilUrl = "";
        private string StoreApilUrl = "";
        private readonly IWebHostEnvironment _env;

        public AccountController(IWebHostEnvironment env)
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            UserApilUrl = "http://localhost:5208/api/User";
            StoreApilUrl = "http://localhost:5208/api/Store";
            _env = env;
        }

        // Configure firebase
        private static string ApiKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("firebaseConfig")["apiKey"];
        private static string Bucket = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("firebaseConfig")["storageBucket"];
        private static string AuthEmail = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("firebaseConfig")["authEmail"];
        private static string AuthPassword = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("firebaseConfig")["authPassword"];


        [Authorize]
        public async Task<IActionResult> Index()
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }

            // GET User Detail
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.GetAsync(UserApilUrl + "/StoreProfile");
            if ((int)response.StatusCode != 200)
            {
                return RedirectToAction("Index");
            }
            string strData = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(strData);
            APIResult<UserDetailViewDTO> result = data.ToObject<APIResult<UserDetailViewDTO>>();

            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }

            return View(result.ResultObj);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromForm] string name, [FromForm] string phone)
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }

            if (String.IsNullOrEmpty(name) || name.Length > 100)
            {
                TempData["ErrorMessage"] = "Name is required and must be under 100 characters";
                return RedirectToAction("Index");
            }

            var regex = new Regex(@"^(\+84|84|0[1-9]|84[1-9]|\+84[1-9])+([0-9]{8})\b$");
            if (String.IsNullOrEmpty(phone) || !regex.IsMatch(phone))
            {
                TempData["ErrorMessage"] = "Phone is required and must be in valid format: 0123456789 or 84123456789";
                return RedirectToAction("Index");
            }

            // Update profile
            string jsonString = JsonConvert.SerializeObject(new UserProfileUpdateDTO()
            {
                Name = name,
                Phone = phone,
            });
            StringContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.PutAsync(UserApilUrl + "/UpdateProfile", content);
            if ((int)response.StatusCode != 200)
            {
                TempData["ErrorMessage"] = "Cannot update your profile";
                return RedirectToAction("Index");
            }
            string strData = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(strData);
            APIResult<bool> result = data.ToObject<APIResult<bool>>();
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "Update Profile Successfully";
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromForm] string oldPassword, [FromForm] string newPassword
            , [FromForm] string confirmPassword)
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }

            if (String.IsNullOrEmpty(oldPassword) || oldPassword.Length < 8 || oldPassword.Length > 100)
            {
                TempData["ErrorMessage"] = "Old Password is required, must be over 8 characters and under 100 characters";
                return RedirectToAction("Index");
            }

            if (String.IsNullOrEmpty(newPassword) || newPassword.Length < 8 || newPassword.Length > 100)
            {
                TempData["ErrorMessage"] = "New Password is required, must be over 8 characters and under 100 characters";
                return RedirectToAction("Index");
            }

            if (String.IsNullOrEmpty(confirmPassword) || confirmPassword.Length < 8 || confirmPassword.Length > 100)
            {
                TempData["ErrorMessage"] = "Confirm Password is required, must be over 8 characters and under 100 characters";
                return RedirectToAction("Index");
            }

            if (newPassword.Equals(oldPassword))
            {
                TempData["ErrorMessage"] = "New password cannot be the same as old password";
                return RedirectToAction("Index");
            }

            if (!newPassword.Equals(confirmPassword))
            {
                TempData["ErrorMessage"] = "Confirm password must match new password";
                return RedirectToAction("Index");
            }

            // Change password
            string jsonString = JsonConvert.SerializeObject(new UserPasswordUpdateDTO()
            {
                OldPassword = oldPassword,
                NewPassword = newPassword,
                ConfirmPassword = confirmPassword
            });
            StringContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.PutAsync(UserApilUrl + "/ChangePassword", content);
            if ((int)response.StatusCode != 200)
            {
                TempData["ErrorMessage"] = "Cannot change your password";
                return RedirectToAction("Index");
            }
            string strData = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(strData);
            APIResult<bool> result = data.ToObject<APIResult<bool>>();
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "Change Password Successfully";
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateStore([FromForm] string name, [FromForm] string address
            , [FromForm] string description, [FromForm] string logoImage, [FromForm] string coverImage,
            [FromForm] IFormFile? logoImageUpload, [FromForm] IFormFile? coverImageUpload)
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }

            if (String.IsNullOrEmpty(name))
            {
                TempData["ErrorMessage"] = "Store Name is required";
                return RedirectToAction("Index");
            }

            if (String.IsNullOrEmpty(address))
            {
                TempData["ErrorMessage"] = "Store Address is required";
                return RedirectToAction("Index");
            }

            if (String.IsNullOrEmpty(description))
            {
                TempData["ErrorMessage"] = "Store Description is required";
                return RedirectToAction("Index");
            }

            // Upload Image
            // Get Img file
            var fileUploadLogo = logoImageUpload;
            var fileUploadCover = coverImageUpload;

            // Upload Logo img
            if (fileUploadLogo != null && fileUploadLogo.Length > 0)
            {
                logoImage = await UploadImage(fileUploadLogo);
            }

            // Upload Cover img
            if (fileUploadCover != null && fileUploadCover.Length > 0)
            {
                coverImage = await UploadImage(fileUploadCover);
            }

            // Change password
            string jsonString = JsonConvert.SerializeObject(new StoreInformationUpdateDTO()
            {
                Address = address,
                CoverImage = coverImage,
                Description = description,
                LogoImage = logoImage,
                Name = name
            });
            StringContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.PutAsync(StoreApilUrl + "/UpdateStore", content);
            if ((int)response.StatusCode != 200)
            {
                TempData["ErrorMessage"] = "Cannot Update Store Profile";
                return RedirectToAction("Index");
            }
            string strData = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(strData);
            APIResult<bool> result = data.ToObject<APIResult<bool>>();
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "Update Store Profile Successfully";
            return RedirectToAction("Index");
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
