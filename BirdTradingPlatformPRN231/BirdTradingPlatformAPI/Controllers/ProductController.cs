using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Implementation;
using Repository.Interface;
using System.Security.Claims;

namespace BirdTradingPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IProductRepository _productRepository = new ProductRepository();

        // Get publicly available product list with search, filter, sort and pagination
        [HttpGet("Public")]
        public IActionResult GetPublicProductList([FromQuery] int page, [FromQuery] string? name, [FromQuery] long category,
            [FromQuery] long pmin, [FromQuery] long pmax, [FromQuery] int order)
        {
            var result = _productRepository.GetProductsPublic(page, name, category, pmin, pmax, order);

            return Ok(result);
        }

        // Get publicly available product detail with search, filter, sort and pagination
        [HttpGet("Public/{id}")]
        public IActionResult GetPublicProductDetailById(long id)
        {
            var result = _productRepository.GetProductDetailPublicById(id);
            return Ok(result);
        }

        // Get publicly available product list by store id
        [HttpGet("Store/{id}")]
        public IActionResult GetProductsPublicByStoreId(long id)
        {
            var result = _productRepository.GetProductsPublicByStoreId(id);
            return Ok(result);
        }

        // Get product list of a currently logined store
        [HttpGet("CurrentStore")]
        [Authorize]
        public IActionResult GetProductsStore([FromQuery] int page, [FromQuery] string? name, 
            [FromQuery] long category, [FromQuery] long pmin, [FromQuery] long pmax, [FromQuery] int order)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _productRepository.GetProductsStore(page, name, category, pmin, pmax, order, currentUserId);
            return Ok(result);
        }

        // Get product detail of a currently logined store
        [HttpGet("CurrentStore/{id}")]
        [Authorize]
        public IActionResult GetProductDetailStore(long id)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _productRepository.GetProductDetailStore(id, currentUserId);
            return Ok(result);
        }
    }
}
