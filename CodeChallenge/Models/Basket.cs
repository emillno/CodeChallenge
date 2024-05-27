namespace CodeChallenge.Models;

public class Basket
{
    public Guid Id { get; set; }
    public List<BasketItem> Items { get; set; } = new List<BasketItem>();
    public DateTime CreatedAt { get; set; }
}

public class BasketItem
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public int Size { get; set; }
    public int Quantity { get; set; }
}