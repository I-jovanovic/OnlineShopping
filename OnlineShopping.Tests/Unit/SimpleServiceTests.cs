using Xunit;
using FluentAssertions;
using OnlineShopping.Core.Entities;

namespace OnlineShopping.Tests.Unit;

public class SimpleServiceTests
{
    [Fact]
    public void Product_Entity_Should_Initialize_Correctly()
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            Price = 99.99m
        };
        product.Name.Should().Be("Test Product");
        product.Price.Should().Be(99.99m);
    }

    [Fact]
    public void Customer_Entity_Should_Initialize_Correctly()
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };
        customer.FirstName.Should().Be("John");
        customer.LastName.Should().Be("Doe");
        customer.Email.Should().Be("john.doe@example.com");
    }

    [Fact]
    public void Order_Should_Calculate_Total_Correctly()
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = "ORD-001",
            OrderItems = new List<OrderItem>
            {
                new OrderItem { Quantity = 2, UnitPrice = 50m },
                new OrderItem { Quantity = 1, UnitPrice = 100m }
            }
        };

        var total = order.OrderItems.Sum(i => i.Quantity * i.UnitPrice);
        total.Should().Be(200m);
    }
}