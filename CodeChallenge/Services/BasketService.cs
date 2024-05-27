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
    void RemoveProductFromBasket(Basket basket, int productId);
    void UpdateProductQuantity(Basket basket, int productId, int quantity);
    Task<string?> SubmitOrder(Basket basket, string userEmail);
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
        _orderEndpoint = $"{configuration["CodeChallengeApi:BaseUrl"]}/CreateOrder";
    }

    public Guid CreateBasket(Product product)
    {
        var id = Guid.NewGuid();
        var basketItem = new BasketItem
        {
            ProductId = product.Id,
            Name = product.Name,
            Price = product.Price,
            Size = product.Size,
            Quantity = 1
        };
        var basket = new Basket { Id = id, Items = new List<BasketItem> { basketItem }, CreatedAt = DateTime.UtcNow };
        _baskets[id] = basket;
        return id;
    }

    public Basket? GetBasket(Guid id)
    {
        _baskets.TryGetValue(id, out var basket);
        return basket;
    }

    public void AddProductToBasket(Basket basket, Product product)
    {
        var existingItem = basket.Items.Find(i => i.ProductId == product.Id);
        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            basket.Items.Add(new BasketItem
            {
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                Size = product.Size,
                Quantity = 1
            });
        }
    }

    public void RemoveProductFromBasket(Basket basket, int productId)
    {
        var item = basket.Items.Find(i => i.ProductId == productId);
        if (item != null)
        {
            basket.Items.Remove(item);
        }
    }

    public void UpdateProductQuantity(Basket basket, int productId, int quantity)
    {
        var item = basket.Items.Find(i => i.ProductId == productId);
        if (item == null)
        {
            throw new Exception("Product not found in basket");
        }

        if (quantity <= 0)
        {
            basket.Items.Remove(item);
        }
        else
        {
            item.Quantity = quantity;
        }
    }

    public async Task<string?> SubmitOrder(Basket basket, string userEmail)
    {
        var client = _clientFactory.CreateClient();
        var token = await _tokenService.GetTokenAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var orderLines = basket.Items.Select(i => new OrderLine
        {
            ProductId = i.ProductId,
            ProductName = i.Name,
            ProductUnitPrice = i.Price,
            ProductSize = i.Size.ToString(),
            Quantity = i.Quantity,
            TotalPrice = i.Price * i.Quantity
        }).ToList();

        var createOrderRequest = new CreateOrderRequest
        {
            UserEmail = userEmail,
            TotalAmount = orderLines.Sum(ol => ol.TotalPrice),
            OrderLines = orderLines
        };

        var response = await client.PostAsync(
            _orderEndpoint,
            new StringContent(JsonSerializer.Serialize(createOrderRequest), System.Text.Encoding.UTF8, "application/json")
        );

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        var orderResponse = JsonSerializer.Deserialize<JsonElement>(content);
        return orderResponse.GetProperty("orderId").GetString();
    }

    public void RemoveBasket(Guid id)
    {
        _baskets.TryRemove(id, out _);
    }
}