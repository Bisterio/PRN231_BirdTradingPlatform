using BusinessObject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace BirdTradingPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserAccountRepository _userRepository = new UserAccountRepository();

        [HttpPost("Authenticate")]
        [AllowAnonymous]
        public IActionResult Authenticate([FromBody] LoginDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _userRepository.Authenticate(request);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] RegisterDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _userRepository.Register(request);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}
