using CodeChallenge.Models;

namespace CodeChallenge.Services;

public interface IBasketService
{
    Guid CreateBasket(Product product);
    Basket GetBasket(Guid id);
    void AddProductToBasket(Basket basket, Product product);
    void RemoveProductFromBasket(Basket basket, Product product);
    void UpdateProductQuantity(Basket basket, int productId, int quantity);
    Task<bool> SubmitOrder(Basket basket);
    void RemoveBasket(Guid id);
}

public class BasketService : IBasketService
{
    public Guid CreateBasket(Product product)
    {
        throw new NotImplementedException();
    }

    public Basket GetBasket(Guid id)
    {
        throw new NotImplementedException();
    }

    public void AddProductToBasket(Basket basket, Product product)
    {
        throw new NotImplementedException();
    }

    public void RemoveProductFromBasket(Basket basket, Product product)
    {
        throw new NotImplementedException();
    }

    public void UpdateProductQuantity(Basket basket, int productId, int quantity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SubmitOrder(Basket basket)
    {
        throw new NotImplementedException();
    }

    public void RemoveBasket(Guid id)
    {
        throw new NotImplementedException();
    }
}