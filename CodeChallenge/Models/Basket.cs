namespace CodeChallenge.Models;

public class Basket
{
    public Guid Id { get; set; }
    public List<Product> Products { get; set; }
    public DateTime CreatedAt { get; set; }
}