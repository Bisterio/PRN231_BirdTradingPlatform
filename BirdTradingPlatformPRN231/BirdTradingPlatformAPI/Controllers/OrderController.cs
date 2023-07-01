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

        // CUSTOMER: Create a new invoice, sub orders and order items
        [HttpPost]
        [Authorize(Roles = "CUSTOMER")]
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

        // CUSTOMER: Get all orders filter by status of currently logined customer
        [HttpGet("Customer")]
        [Authorize(Roles = "CUSTOMER")]
        public IActionResult GetCurrentUserOrders([FromQuery] int page, [FromQuery] byte status)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _orderRepository.GetCurrentUserOrders(page, status, currentUserId);

            return Ok(result);
        }

        // CUSTOMER: Get an order detail of a currently logined user
        [HttpGet("Customer/{id}")]
        [Authorize(Roles = "CUSTOMER")]
        public IActionResult GetOrderDetailCustomer(long id)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _orderRepository.GetOrderDetailCustomer(id, currentUserId);
            if (result == null) return NotFound("Can't get this order detail!");
            return Ok(result);
        }

        // STORE: Get all orders filter by status of currently logined store
        [HttpGet("Store")]
        [Authorize(Roles = "STORE")]
        public IActionResult GetCurrentStoreOrders([FromQuery] int page, [FromQuery] byte status, [FromQuery] string? orderIdSearch)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentStoreId = long.Parse(idString);

            var result = _orderRepository.GetCurrentStoreOrders(page, status, currentStoreId, orderIdSearch);

            return Ok(result);
        }

        // Get an order detail of a currently logined store
        [HttpGet("Store/{id}")]
        [Authorize(Roles = "STORE")]
        public IActionResult GetOrderDetailStore(long id)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentStoreId = long.Parse(idString);

            var result = _orderRepository.GetOrderDetailStore(id, currentStoreId);
            if (result == null) return NotFound("Can't get this order detail!");
            return Ok(result);
        }

        // CUSTOMER: Cancel an order detail of a currently logined user *** doi thanh post kem voi cancel reason ***
        //[HttpGet("Customer/Cancel/{id}")]
        //[Authorize(Roles = "CUSTOMER")]
        //public IActionResult CancelOrderDetailCustomer(long id)
        //{
        //    var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (String.IsNullOrEmpty(idString)) return Unauthorized();
        //    long currentUserId = long.Parse(idString);

        //    var result = _orderRepository.CancelOrderDetailCustomer(id, currentUserId);
        //    if (result == null) return NotFound("Can't cancel this order!");
        //    return Ok(result);
        //}
    }
}
