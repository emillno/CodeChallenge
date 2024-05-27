using CodeChallenge.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("TopRankedProducts")]
        public async Task<IActionResult> GetTopRankedProducts()
        {
            var products = await _productService.GetTopRankedProducts();
            return Ok(products);
        }

        [HttpGet("ProductsSortedByPrice")]
        public async Task<IActionResult> GetProductsSortedByPrice(int page, int pageSize)
        {
            if (pageSize > 1000)
            {
                return BadRequest("Page size cannot exceed 1000.");
            }

            var products = await _productService.GetSortedProductsByPrice(page, pageSize);
            return Ok(products);
        }

        [HttpGet("CheapestProducts")]
        public async Task<IActionResult> GetCheapestProducts()
        {
            var products = await _productService.GetCheapestProducts();
            return Ok(products);
        }
    }
}