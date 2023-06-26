using BusinessObject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository;
using System.Security.Claims;

namespace BirdTradingPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IOrderRepository _orderRepository = new OrderRepository();

        [HttpPost]
        [Authorize]
        public IActionResult CreateNewOrders([FromBody] OrderCreateDTO newOrders)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Select(x => x.Value?.Errors)
                    .Where(y => y?.Count > 0)
                    .ToList();
                return BadRequest(errors[0]);
            }

            var result = _orderRepository.CreateNewOrders(newOrders, currentUserId);

            return Ok(result);
        }

        [HttpGet("Customer")]
        [Authorize]
        public IActionResult GetCurrentUserOrders([FromQuery] string? name, [FromQuery] byte status)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);


            var result = "";

            return Ok(result);
        }
    }
}
