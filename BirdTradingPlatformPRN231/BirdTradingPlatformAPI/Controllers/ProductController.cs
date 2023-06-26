using BusinessObject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace BirdTradingPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IProductRepository _productRepository = new ProductRepository();

        [HttpGet("Public")]
        public IActionResult GetPublicProductList([FromQuery] int page, [FromQuery] string? name, [FromQuery] long category,
            [FromQuery] long pmin, [FromQuery] long pmax, [FromQuery] int order)
        {
            var result = _productRepository.GetProductsPublic(page, name, category, pmin, pmax, order);

            return Ok(result);
        }

        [HttpGet("Public/{id}")]
        public IActionResult GetPublicProductDetailById(long id)
        {
            var result = _productRepository.GetProductDetailPublicById(id);
            return Ok(result);
        }

        [HttpGet("Store/{id}")]
        public IActionResult GetPublicProductDetailByStore(long id)
        {
            var result = _productRepository.GetProductsPublicByStoreId(id);
            return Ok(result);
        }
    }
}
