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
    public class StoreController : ControllerBase
    {
        private IStoreRepository _storeRepository = new StoreRepository();

        // Get detail of a store and its available products
        [HttpGet("Public/{id}")]
        [AllowAnonymous]
        public IActionResult GetPublicStoreDetailById(long id)
        {
            var result = _storeRepository.GetPublicStoreDetailById(id);
            return Ok(result);
        }

        // Get list of all available store
        [HttpGet("Public")]
        [AllowAnonymous]
        public IActionResult GetPublicStoreList()
        {
            var result = _storeRepository.GetStoresPublic();

            return Ok(result);
        }

        [HttpPut("UpdateStore")]
        [Authorize(Roles = "STORE")]
        public IActionResult UpdateStore([FromBody] StoreInformationUpdateDTO info)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _storeRepository.UpdateStore(currentUserId, info);

            return Ok(result);
        }
    }
}
