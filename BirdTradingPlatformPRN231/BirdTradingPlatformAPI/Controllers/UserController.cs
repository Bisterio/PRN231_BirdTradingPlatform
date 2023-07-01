using BusinessObject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Implementation;
using Repository.Interface;

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
    }
}
