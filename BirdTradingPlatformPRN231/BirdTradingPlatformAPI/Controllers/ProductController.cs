using BusinessObject.DTOs;
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

        // PUBLIC: Get publicly available product list with search, filter, sort and pagination
        [HttpGet("Public")]
        [AllowAnonymous]
        public IActionResult GetPublicProductList([FromQuery] int page, [FromQuery] string? name, [FromQuery] long category,
            [FromQuery] long pmin, [FromQuery] long pmax, [FromQuery] int order)
        {
            var result = _productRepository.GetProductsPublic(page, name, category, pmin, pmax, order);

            return Ok(result);
        }

        // PUBLIC: Get publicly available product detail with search, filter, sort and pagination
        [HttpGet("Public/{id}")]
        [AllowAnonymous]
        public IActionResult GetPublicProductDetailById(long id)
        {
            var result = _productRepository.GetProductDetailPublicById(id);
            return Ok(result);
        }

        // PUBLIC: Get publicly available product list by store id
        [HttpGet("Store/{id}")]
        [AllowAnonymous]
        public IActionResult GetProductsPublicByStoreId(long id)
        {
            var result = _productRepository.GetProductsPublicByStoreId(id);
            return Ok(result);
        }

        // STORE: Get product list of a currently logined store
        [HttpGet("CurrentStore")]
        [Authorize(Roles = "STORE")]
        public IActionResult GetProductsStore([FromQuery] int page, [FromQuery] string? name, 
            [FromQuery] long category, [FromQuery] long pmin, [FromQuery] long pmax, [FromQuery] int order)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _productRepository.GetProductsStore(page, name, category, pmin, pmax, order, currentUserId);
            if(result == null) return NotFound();
            return Ok(result);
        }

        // STORE: Get product detail of a currently logined store
        [HttpGet("CurrentStore/{id}")]
        [Authorize(Roles = "STORE")]
        public IActionResult GetProductDetailStore(long id)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            var result = _productRepository.GetProductDetailStore(id, currentUserId);
            return Ok(result);
        }

        // STORE: Add a new product
        [HttpPost]
        [Authorize(Roles = "STORE")]
        public IActionResult AddProduct([FromBody] ProductCreateDTO product)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _productRepository.AddProduct(product, currentUserId);

            return Ok(result);
        }

        // STORE: Edit a product of store by id
        [HttpPut("{id}")]
        [Authorize(Roles = "STORE")]
        public IActionResult EditProduct(long id, [FromBody] ProductCreateDTO product)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _productRepository.EditProduct(id, product, currentUserId);

            return Ok(result);
        }

        // STORE: Delete a product by id
        [HttpDelete("{id}")]
        [Authorize(Roles = "STORE")]
        public IActionResult DeleteProduct(long id)
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(idString)) return Unauthorized();
            long currentUserId = long.Parse(idString);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _productRepository.DeleteProduct(id, currentUserId);

            return Ok(result);
        }

        // CUSTOMER: Check for shipping cost and item's valid
        [HttpPost("CalculateShip")]
        //[Authorize(Roles = "CUSTOMER")]
        public async Task <IActionResult> CheckShippingCost([FromBody] CartAddressDTO request)
        {
            var result = await _productRepository.CheckShippingCost(request);

            return Ok(result);
        }
    }
}
