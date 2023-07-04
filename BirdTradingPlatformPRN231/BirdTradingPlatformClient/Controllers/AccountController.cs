using BusinessObject.Common;
using BusinessObject.DTOs;
using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace BirdTradingPlatformClient.Controllers
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
        public async Task<IActionResult> Index()
        {
            // Check for valid jwt token
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Logout", "Home");
            }

            // GET User Detail
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
            HttpResponseMessage response = await client.GetAsync(UserApilUrl + "/Profile");
            if ((int)response.StatusCode != 200)
            {
                return RedirectToAction("Index");
            }
            string strData = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(strData);
            APIResult<UserProfileViewDTO> result = data.ToObject<APIResult<UserProfileViewDTO>>();

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
    }
}
