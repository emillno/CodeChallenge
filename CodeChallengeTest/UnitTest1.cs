using CodeChallenge.Controllers;
using CodeChallenge.Models;
using CodeChallenge.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CodeChallangeTest;

public class BasketControllerTests
{
    private readonly Mock<IBasketService> _basketServiceMock;
    private readonly Mock<IProductService> _productServiceMock;
    private readonly BasketController _controller;

    public BasketControllerTests()
    {
        _basketServiceMock = new Mock<IBasketService>();
        _productServiceMock = new Mock<IProductService>();
        _controller = new BasketController(_basketServiceMock.Object, _productServiceMock.Object);
    }

    [Fact]
    public void CreateBasket_ShouldReturnBasketId_WhenProductExists()
    {
        // Arrange
        var productId = 1;
        var product = new Product { Id = productId, Name = "Test Product", Price = 10.0, Size = 1, Stars = 5 };
        _productServiceMock.Setup(s => s.GetTopRankedProducts()).ReturnsAsync(new List<Product> { product });

        var basketId = Guid.NewGuid();
        _basketServiceMock.Setup(s => s.CreateBasket(It.IsAny<Product>())).Returns(basketId);

        // Act
        var result = _controller.CreateBasket(productId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }
}