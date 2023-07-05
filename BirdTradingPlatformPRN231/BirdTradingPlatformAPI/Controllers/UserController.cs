using BusinessObject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Implementation;
using Repository.Interface;
using System.Security.Claims;

namespace BirdTradingPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserAccountRepository _userRepository = new UserAccountRepository();

        [HttpPost("AuthenticateCustomer")]
        [AllowAnonymous]
        public IActionResult AuthenticateCustomer([FromBody] LoginDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _userRepository.AuthenticateCustomer(request);

            return Ok(result);
        }

        [HttpPost("AuthenticateStore")]
        [AllowAnonymous]
        public IActionResult AuthenticateStore([FromBody] LoginDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _userRepository.AuthenticateStore(request);

            return Ok(result);
        }

        [HttpPost("AuthenticateAdmin")]
        [AllowAnonymous]
        public IActionResult AuthenticateAdmin([FromBody] LoginDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _userRepository.AuthenticateAdmin(request);

            return Ok(result);
        }

        [HttpPost("RegisterCustomer")]
        [AllowAnonymous]
        public IActionResult RegisterCustomer([FromBody] RegisterCustomerDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _userRepository.RegisterCustomer(request);

            return Ok(result);
        }

        [HttpPost("RegisterStore")]
        [AllowAnonymous]
        public IActionResult RegisterStore([FromBody] RegisterStoreDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _userRepository.RegisterStore(request);

            return Ok(result);
        }

        [HttpGet("All")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetAllUsers([FromQuery] int page, [FromQuery] string? roleSearch)
        {
            var result = _userRepository.GetAllUsers(page, roleSearch);

            return Ok(result);
        }

        [HttpGet("Detail/{id}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetUserDetail(long id)
        {
            var result = _userRepository.GetUserDetail(id);

            return Ok(result);
        }

        [HttpPut("ChangeStatus/{id}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult ChangeStatus(long id)
        {
            var result = _userRepository.ChangeStatus(id);

            return Ok(result);
        }

        [HttpGet("Profile")]
        [Authorize(Roles = "CUSTOMER")]
        public IActionResult GetCurrentCustomer()
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _userRepository.GetCurrentCustomer(currentUserId);

            return Ok(result);
        }

        [HttpPut("UpdateProfile")]
        [Authorize]
        public IActionResult UpdateProfile([FromBody] UserProfileUpdateDTO profile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _userRepository.UpdateProfile(currentUserId, profile);

            return Ok(result);
        }

        [HttpPut("ChangePassword")]
        [Authorize]
        public IActionResult ChangePassword([FromBody] UserPasswordUpdateDTO password)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _userRepository.ChangePassword(currentUserId, password);

            return Ok(result);
        }
    }
}
