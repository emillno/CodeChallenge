using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Text.Json;
using CodeChallenge.Models;

namespace CodeChallenge.Services;

public interface IBasketService
{
    Guid CreateBasket(Product product);
    Basket? GetBasket(Guid id);
    void AddProductToBasket(Basket basket, Product product);
    void RemoveProductFromBasket(Basket basket, Product product);
    void UpdateProductQuantity(Basket basket, int productId, int quantity);
    Task<bool> SubmitOrder(Basket basket, string userEmail);
    void RemoveBasket(Guid id);
}

public class BasketService : IBasketService
{
    private readonly ConcurrentDictionary<Guid, Basket> _baskets = new ConcurrentDictionary<Guid, Basket>();
    private readonly IHttpClientFactory _clientFactory;
    private readonly ITokenService _tokenService;
    private readonly string _orderEndpoint;

    public BasketService(IHttpClientFactory clientFactory, ITokenService tokenService, IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _tokenService = tokenService;
        _orderEndpoint = $"{configuration["CodeChallengeApi:BaseUrl"]}/api/CreateOrder";
    }

    public Guid CreateBasket(Product product)
    {
        var id = Guid.NewGuid();
        var basket = new Basket { Id = id, Products = new List<Product> { product }, CreatedAt = DateTime.UtcNow };
        _baskets[id] = basket;
        return id;
    }

    public Basket? GetBasket(Guid id)
    {
        _baskets.TryGetValue(id, out var basket);
        return basket;
    }

    public void AddProductToBasket(Basket basket, Product product) 
        => basket.Products.Add(product);

    public void RemoveProductFromBasket(Basket basket, Product product) 
        => basket.Products.Remove(product);

    public void UpdateProductQuantity(Basket basket, int productId, int quantity)
        {
            var product = basket.Products.Find(p => p.Id == productId);
            if (product == null)
            {
                throw new Exception("Product not found in basket");
            }

            var currentCount = basket.Products.Count(p => p.Id == productId);
            if (quantity <= 0)
            {
                basket.Products.RemoveAll(p => p.Id == productId);
            }
            else if (quantity > currentCount)
            {
                var additionalProducts = Enumerable.Repeat(product, quantity - currentCount);
                basket.Products.AddRange(additionalProducts);
            }
            else
            {
                var removeCount = currentCount - quantity;
                for (int i = 0; i < removeCount; i++)
                {
                    basket.Products.Remove(product);
                }
            }
        }

        public async Task<bool> SubmitOrder(Basket basket, string userEmail)
        {
            var client = _clientFactory.CreateClient();
            var token = await _tokenService.GetTokenAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var orderLines = basket.Products.GroupBy(p => p.Id).Select(g => new OrderLine
            {
                ProductId = g.Key,
                ProductName = g.First().Name,
                ProductUnitPrice = g.First().Price,
                ProductSize = g.First().Size.ToString(),
                Quantity = g.Count(),
                TotalPrice = g.Sum(p => p.Price)
            }).ToList();

            var totalAmount = orderLines.Sum(ol => ol.TotalPrice);

            var createOrderRequest = new CreateOrderRequest
            {
                UserEmail = userEmail,
                TotalAmount = totalAmount,
                OrderLines = orderLines
            };

            var response = await client.PostAsync(
                _orderEndpoint,
                new StringContent(JsonSerializer.Serialize(createOrderRequest), System.Text.Encoding.UTF8, "application/json")
            );

            return response.IsSuccessStatusCode;
        }

        public void RemoveBasket(Guid id) 
            => _baskets.TryRemove(id, out _);
}