using BusinessObject.DTOs;
using BusinessObject.Models;
using DataAccess;
using MailKit.Search;
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
        private IMailRepository _mailRepository;

        public OrderController(IMailRepository mailRepository)
        {
            _mailRepository = mailRepository;
        }

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

            if (result.IsSuccess == true)
            {
                var mail = new MailRequest()
                {
                    ToEmail = newOrders.Email,
                    Subject = $"Your order has been created successfully.",
                    Body = $"Your order has been created successfully. Here is your invoice."
                };

                _mailRepository.SendEmailAsync(mail);
            }

            return Ok(result);
        }
        // ADMIN: Get all orders filter by isReported
        [HttpGet("Admin")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetCurrentAdminOrders([FromQuery] int page, [FromQuery] byte isReported)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();

            var result = _orderRepository.GetCurrentAdminOrders(page, isReported);

            return Ok(result);
        }
        // ADMIN: Get an order detail
        [HttpGet("Admin/{id}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetOrderDetailAdmin(long id)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();

            var result = _orderRepository.GetOrderDetailAdmin(id);
            if (result == null) return NotFound("Can't get this order detail!");
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
        [HttpPut("CustomerCancel/{id}")]
        [Authorize(Roles = "CUSTOMER")]
        public IActionResult CancelOrderDetailCustomer(long id, [FromBody] string cancelReason)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _orderRepository.CancelOrderDetailCustomer(id, currentUserId, cancelReason);
            if (result == null) return NotFound("Can't cancel this order!");

            var order = _orderRepository.GetOrderForEmail(id);

            if (result.IsSuccess == true)
            {
                var mail = new MailRequest()
                {
                    ToEmail = order.Invoice.Email,
                    Subject = $"You have cancelled your order #{id}.",
                    Body = $"You have cancelled your order #{id}."
                };

                _mailRepository.SendEmailAsync(mail);
            }

            return Ok(result);
        }

        //CUSTOMER: Send cancel request of an order detail of currently logined user
        [HttpPut("CancelRequest/{id}")]
        [Authorize(Roles = "CUSTOMER")]
        public IActionResult RequestCancelOrderDetailCustomer(long id, [FromBody] string cancelReason)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _orderRepository.RequestCancelOrderDetailCustomer(id, currentUserId, cancelReason);
            if (result == null) return NotFound("Can't cancel this order!");
            return Ok(result);
        }

        [HttpPut("ApproveOrder/{id}")]
        [Authorize(Roles = "STORE")]
        public IActionResult ApproveOrderStore(long id)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _orderRepository.ApproveOrderStore(id, currentUserId);
            if (result == null) return NotFound("Can't approve this order!");

            var order = _orderRepository.GetOrderForEmail(id);

            if (result.IsSuccess == true)
            {
                var mail = new MailRequest()
                {
                    ToEmail = order.Invoice.Email,
                    Subject = $"Your order #{id} has been approved.",
                    Body = $"Your order #{id} has been approved. Please wait."
                };

                _mailRepository.SendEmailAsync(mail);

            }

            return Ok(result);
        }

        [HttpPut("StoreCancel/{id}")]
        [Authorize(Roles = "STORE")]
        public IActionResult CancelOrderDetailStore(long id, [FromBody] string cancelReason)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _orderRepository.CancelOrderDetailStore(id, currentUserId, cancelReason);
            if (result == null) return NotFound("Can't cancel this order!");

            var order = _orderRepository.GetOrderForEmail(id);

            if (result.IsSuccess == true)
            {
                var mail = new MailRequest()
                {
                    ToEmail = order.Invoice.Email,
                    Subject = $"Your order #{id} has been cancelled.",
                    Body = $"Your order #{id} has been cancelled. Due to some reasons."
                };

                _mailRepository.SendEmailAsync(mail);
            }

            return Ok(result);
        }

        [HttpPut("CancelApprove/{id}")]
        [Authorize(Roles = "STORE")]
        public IActionResult ApproveOrderCancelRequestStore(long id)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _orderRepository.ApproveOrderCancelRequestStore(id, currentUserId);
            if (result == null) return NotFound("Can't approve this request!");

            var order = _orderRepository.GetOrderForEmail(id);

            if (result.IsSuccess == true)
            {
                var mail = new MailRequest()
                {
                    ToEmail = order.Invoice.Email,
                    Subject = $"Your order #{id} request for cancel is approved.",
                    Body = $"Your order #{id} request for cancel is approved."
                };

                _mailRepository.SendEmailAsync(mail);
            }

            return Ok(result);
        }

        [HttpPut("CancelDeclined/{id}")]
        [Authorize(Roles = "STORE")]
        public IActionResult DeclineOrderCancelRequestStore(long id)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _orderRepository.DeclineOrderCancelRequestStore(id, currentUserId);
            if (result == null) return NotFound("Can't approve this request!");

            var order = _orderRepository.GetOrderForEmail(id);

            if (result.IsSuccess == true)
            {
                var mail = new MailRequest()
                {
                    ToEmail = order.Invoice.Email,
                    Subject = $"Your order #{id} request for cancel is declined.",
                    Body = $"Your order #{id} request for cancel is declined. Due to some reasons."
                };

                _mailRepository.SendEmailAsync(mail);
            }

            return Ok(result);
        }

        [HttpPut("Deliver/{id}")]
        [Authorize(Roles = "STORE")]
        public IActionResult DeliverOrder(int id)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _orderRepository.DeliverOrder(id, currentUserId);
            if (result == null) return NotFound("Can't deliver this order.");

            var order = _orderRepository.GetOrderForEmail(id);

            if (result.IsSuccess == true)
            {
                var mail = new MailRequest()
                {
                    ToEmail = order.Invoice.Email,
                    Subject = $"Your order #{id} is now delivering.",
                    Body = $"Your order #{id} is now delivering. Please wait."
                };

                _mailRepository.SendEmailAsync(mail);
            }

            return Ok(result);
        }

        [HttpPut("Complete/{id}")]
        [Authorize(Roles = "STORE")]
        public IActionResult ConfirmOrderDelivered(int id)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _orderRepository.ConfirmOrderDelivered(id, currentUserId);
            if (result == null) return NotFound("Can't confirm this order's delivery.");

            var order = _orderRepository.GetOrderForEmail(id);

            if (result.IsSuccess == true)
            {
                var mail = new MailRequest()
                {
                    ToEmail = order.Invoice.Email,
                    Subject = $"Your order #{id} is successfully delivered.",
                    Body = $"Your order #{id} is successfully delivered. Thank you for your purchase."
                };

                _mailRepository.SendEmailAsync(mail);
            }

            return Ok(result);
        }

        [HttpPut("RefundRequest/{id}")]
        [Authorize(Roles = "CUSTOMER")]
        public IActionResult RefundRequest(int id, [FromBody] string refundReason)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _orderRepository.RefundRequest(id, currentUserId, refundReason);
            if (result == null) return NotFound("Can't request this order's refund.");

            return Ok(result);
        }

        [HttpPut("RefundDecline/{id}")]
        [Authorize(Roles = "STORE")]
        public IActionResult RefundDecline(int id)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _orderRepository.RefundDecline(id, currentUserId);
            if (result == null) return NotFound("Can't decline this order's refund request.");

            var order = _orderRepository.GetOrderForEmail(id);

            if (result.IsSuccess == true)
            {
                var mail = new MailRequest()
                {
                    ToEmail = order.Invoice.Email,
                    Subject = $"Your order #{id} request for refund is declined.",
                    Body = $"Your order #{id} request for refund is declined. Due to some reasons."
                };

                _mailRepository.SendEmailAsync(mail);
            }

            return Ok(result);
        }

        [HttpPut("RefundAccept/{id}")]
        [Authorize(Roles = "STORE")]
        public IActionResult RefundAccept(int id)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _orderRepository.RefundAccept(id, currentUserId);
            if (result == null) return NotFound("Can't accept this order's refund request.");

            var order = _orderRepository.GetOrderForEmail(id);

            if (result.IsSuccess == true)
            {
                var mail = new MailRequest()
                {
                    ToEmail = order.Invoice.Email,
                    Subject = $"Your order #{id} request for refund is approved.",
                    Body = $"Your order #{id} request for refund is approved. Due to some reasons."
                };

                _mailRepository.SendEmailAsync(mail);
            }

            return Ok(result);
        }

        [HttpPut("Report/{id}")]
        [Authorize(Roles = "CUSTOMER")]
        public IActionResult Report(int id, [FromBody] string reportReason)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _orderRepository.Report(id, currentUserId, reportReason);
            if (result == null) return NotFound("Can't accept this order's report.");

            return Ok(result);
        }

        [HttpPut("ResolveReport/{id}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult ResolveReport(int id)
        {
            var result = _orderRepository.ResolveReport(id);
            if (result == null) return NotFound("Can't resolve this order's report.");

            return Ok(result);
        }

        [HttpPut("ApproveRefundReport/{id}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult ApproveRefundReport(int id)
        {
            var result = _orderRepository.ApproveRefundReport(id);
            if (result == null) return NotFound("Can't resolve accept this order's report.");

            return Ok(result);
        }
    }
}
