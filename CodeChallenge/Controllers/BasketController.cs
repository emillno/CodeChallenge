using CodeChallenge.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeChallenge.Controllers;

[ApiController]
[Route("[controller]")]
public class BasketController : ControllerBase
{
    private readonly IBasketService _basketService;
    private readonly IProductService _productService;

    public BasketController(IBasketService basketService, IProductService productService)
    {
        _basketService = basketService;
        _productService = productService;
    }
    
}