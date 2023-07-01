using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Implementation;
using Repository.Interface;
using System.Data;
using System.Security.Claims;

namespace BirdTradingPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private ICategoryRepository _categoryRepository = new CategoryRepository();

        // PUBLIC: Get  all categories
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetCategories()
        {
            var result = _categoryRepository.GetAllCategories();
            return Ok(result);
        }
    }
}
