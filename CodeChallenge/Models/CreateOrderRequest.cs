namespace CodeChallenge.Models;

public class CreateOrderRequest
{
    public string UserEmail { get; set; }
    public double TotalAmount { get; set; }
    public List<OrderLine> OrderLines { get; set; }
}

public class OrderLine
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public double ProductUnitPrice { get; set; }
    public string ProductSize { get; set; }
    public int Quantity { get; set; }
    public double TotalPrice { get; set; }
}
