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
    public Task<List<Product>> GetTopRankedProducts()
    {
        throw new NotImplementedException();
    }

    public Task<List<Product>> GetSortedProductsByPrice(int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Task<List<Product>> GetCheapestProducts()
    {
        throw new NotImplementedException();
    }
}