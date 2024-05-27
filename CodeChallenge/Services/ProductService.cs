using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CodeChallenge.Models;

namespace CodeChallenge.Services;

public interface IProductService
{
    Task<List<Product>> GetTopRankedProducts();
    Task<List<Product>> GetSortedProductsByPrice(int page, int pageSize);
    Task<List<Product>> GetCheapestProducts();
}

public class ProductService : IProductService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ITokenService _tokenService;
    private readonly string _baseUrl;

    public ProductService(IHttpClientFactory clientFactory, ITokenService tokenService, IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _tokenService = tokenService;
        _baseUrl = $"{configuration["CodeChallengeApi:BaseUrl"]}";
    }

    public async Task<List<Product>> GetTopRankedProducts()
    {
        var products = await GetAllProductsAsync();
        return products.OrderByDescending(p => p.Stars).Take(100).ToList();
    }

    public async Task<List<Product>> GetSortedProductsByPrice(int page, int pageSize)
    {
        var products = await GetAllProductsAsync();
        return products.OrderBy(p => p.Price).Skip((page - 1) * pageSize).Take(pageSize).ToList();
    }

    public async Task<List<Product>> GetCheapestProducts()
    {
        var products = await GetAllProductsAsync();
        return products.OrderBy(p => p.Price).Take(10).ToList();
    }

    private async Task<List<Product>> GetAllProductsAsync()
    {
        var client = _clientFactory.CreateClient();
        var token = await _tokenService.GetTokenAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.GetAsync(_baseUrl + "/GetAllProducts");
        response.EnsureSuccessStatusCode();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var content = await response.Content.ReadAsStringAsync();
        var products = JsonSerializer.Deserialize<List<Product>>(content, options);

        if (products == null) throw new Exception("Failed to deserialize products.");

        return products;
    }
}