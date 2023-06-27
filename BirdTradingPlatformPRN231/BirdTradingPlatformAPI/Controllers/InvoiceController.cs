using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Implementation;
using Repository.Interface;
using System.Security.Claims;

namespace BirdTradingPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private IInvoiceRepository _invoiceRepository = new InvoiceRepository();

        // Get all invoices of currently logined user
        [HttpGet("Customer")]
        [Authorize]
        public IActionResult GetCurrentUserInvoices()
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _invoiceRepository.GetCurrentUserInvoices(currentUserId);

            return Ok(result);
        }

        // Get an invoice detail of a currently logined user
        [HttpGet("Customer/{id}")]
        [Authorize]
        public IActionResult GetInvoiceDetailCustomer(long id)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _invoiceRepository.GetInvoiceDetailCustomer(id, currentUserId);
            if (result == null) return NotFound("Can't get this invoice detail!");
            return Ok(result);
        }
    }
}
