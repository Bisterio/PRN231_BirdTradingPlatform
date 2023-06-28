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
    public class OrderController : ControllerBase
    {
        private IOrderRepository _orderRepository = new OrderRepository();

        // Create a new invoice, sub orders and order items
        [HttpPost]
        [Authorize]
        public IActionResult CreateNewOrders([FromBody] OrderCreateDTO newOrders)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _orderRepository.CreateNewOrders(newOrders, currentUserId);

            return Ok(result);
        }

        // Get all orders filter by status of currently logined user
        [HttpGet("Customer")]
        [Authorize]
        public IActionResult GetCurrentUserOrders([FromQuery] int page, [FromQuery] byte status)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _orderRepository.GetCurrentUserOrders(page, status, currentUserId);

            return Ok(result);
        }

        // Get an order detail of a currently logined user
        [HttpGet("Customer/{id}")]
        [Authorize]
        public IActionResult GetOrderDetailCustomer(long id)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _orderRepository.GetOrderDetailCustomer(id, currentUserId);
            if(result == null) return NotFound("Can't get this order detail!");
            return Ok(result);
        }

        // Cancel an order detail of a currently logined user
        [HttpGet("Customer/Cancel/{id}")]
        [Authorize]
        public IActionResult CancelOrderDetailCustomer(long id)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _orderRepository.CancelOrderDetailCustomer(id, currentUserId);
            if (result == null) return NotFound("Can't cancel this order!");
            return Ok(result);
        }
    }
}
