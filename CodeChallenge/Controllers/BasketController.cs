using CodeChallenge.Models;
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

    [HttpPost("create")]
    public IActionResult CreateBasket([FromBody] int productId)
    {
        var product = _productService.GetTopRankedProducts().Result.Find(p => p.Id == productId);
        if (product == null)
            return NotFound("Product not found");

        var basketId = _basketService.CreateBasket(product);
        return Ok(new { BasketId = basketId });
    }

    [HttpGet("{id}")]
    public IActionResult GetBasket(Guid id)
    {
        var basket = _basketService.GetBasket(id);
        if (basket == null)
            return NotFound();
        
        return Ok(basket);
    }

    [HttpPost("{id}/add")]
    public IActionResult AddProductToBasket(Guid id, [FromBody] int productId)
    {
        var basket = _basketService.GetBasket(id);
        if (basket == null)
            return NotFound();

        var product = _productService.GetTopRankedProducts().Result.Find(p => p.Id == productId);
        if (product == null) 
            return NotFound("Product not found");

        _basketService.AddProductToBasket(basket, product);
        return Ok(basket);
    }

    [HttpPost("{id}/remove")]
    public IActionResult RemoveProductFromBasket(Guid id, [FromBody] int productId)
    {
        var basket = _basketService.GetBasket(id);
        if (basket == null) 
            return NotFound();

        _basketService.RemoveProductFromBasket(basket, productId);
        return Ok(basket);
    }

    [HttpPost("{id}/update")]
    public IActionResult UpdateProductQuantity(Guid id, [FromBody] UpdateProductQuantityRequest request)
    {
        var basket = _basketService.GetBasket(id);
        if (basket == null) 
            return NotFound();

        _basketService.UpdateProductQuantity(basket, request.ProductId, request.Quantity);
        return Ok(basket);
    }

    [HttpPost("{id}/submit")]
    public async Task<IActionResult> SubmitOrder(Guid id, [FromBody] SubmitOrderRequest request)
    {
        var basket = _basketService.GetBasket(id);
        if (basket == null) 
            return NotFound();

        var orderId = await _basketService.SubmitOrder(basket, request.UserEmail);
        if (orderId == null) 
            return StatusCode(500, "Failed to submit order");
        
        _basketService.RemoveBasket(id);
        return Ok(new { OrderId = orderId });

    }
}

public class UpdateProductQuantityRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class SubmitOrderRequest
{
    public string UserEmail { get; set; }
}