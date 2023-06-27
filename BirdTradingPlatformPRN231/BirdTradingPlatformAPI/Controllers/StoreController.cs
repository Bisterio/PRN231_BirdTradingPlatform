using Microsoft.AspNetCore.Mvc;
using Repository.Implementation;
using Repository.Interface;

namespace BirdTradingPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private IStoreRepository _storeRepository = new StoreRepository();

        // Get detail of a store and its available products
        [HttpGet("Public/{id}")]
        public IActionResult GetPublicStoreDetailById(long id)
        {
            var result = _storeRepository.GetPublicStoreDetailById(id);
            return Ok(result);
        }

        [HttpGet("Public")]
        public IActionResult GetPublicProductList()
        {
            var result = _storeRepository.GetStoresPublic();

            return Ok(result);
        }
    }
}
