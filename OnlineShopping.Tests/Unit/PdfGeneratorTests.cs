using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OnlineShopping.Core.Entities;
using OnlineShopping.Infrastructure.Services;

namespace OnlineShopping.Tests.Unit;

public class PdfGeneratorTests
{
    private readonly Mock<ILogger<PdfGeneratorService>> _loggerMock;
    private readonly PdfGeneratorService _pdfGeneratorService;

    public PdfGeneratorTests()
    {
        _loggerMock = new Mock<ILogger<PdfGeneratorService>>();
        _pdfGeneratorService = new PdfGeneratorService(_loggerMock.Object);
    }

    [Fact]
    public void GenerateTransactionReport_WithValidCustomerAndOrders_CreatesPdf()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        var orders = new List<Order>
        {
            new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD-2024-001",
                OrderDate = DateTime.UtcNow.AddDays(-10),
                TotalAmount = 250.50m
            },
            new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD-2024-002",
                OrderDate = DateTime.UtcNow.AddDays(-5),
                TotalAmount = 150.75m
            }
        };

        var reportMonth = DateTime.UtcNow;
        var pdfBytes = _pdfGeneratorService.GenerateTransactionReport(customer, orders, reportMonth);
        pdfBytes.Should().NotBeNull();
        pdfBytes.Should().NotBeEmpty();
        pdfBytes.Length.Should().BeGreaterThan(100, "PDF should have reasonable size");
        
        // Verify PDF header (PDF files start with %PDF)
        var header = System.Text.Encoding.UTF8.GetString(pdfBytes.Take(4).ToArray());
        header.Should().Be("%PDF", "Generated file should be a valid PDF");
    }

    [Fact]
    public void GenerateTransactionReport_WithNoOrders_StillCreatesPdf()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com"
        };

        var emptyOrderList = new List<Order>();
        var reportMonth = DateTime.UtcNow;
        var pdfBytes = _pdfGeneratorService.GenerateTransactionReport(customer, emptyOrderList, reportMonth);
        pdfBytes.Should().NotBeNull();
        pdfBytes.Should().NotBeEmpty();
        
        // Verify it's still a valid PDF even with no orders
        var header = System.Text.Encoding.UTF8.GetString(pdfBytes.Take(4).ToArray());
        header.Should().Be("%PDF");
    }

    [Fact]
    public void GenerateTransactionReport_VerifyTotalCalculation()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "Customer",
            Email = "test@example.com"
        };

        var orders = new List<Order>
        {
            new Order { OrderNumber = "001", OrderDate = DateTime.UtcNow, TotalAmount = 100.00m },
            new Order { OrderNumber = "002", OrderDate = DateTime.UtcNow, TotalAmount = 200.00m },
            new Order { OrderNumber = "003", OrderDate = DateTime.UtcNow, TotalAmount = 300.00m }
        };

        var expectedTotal = orders.Sum(o => o.TotalAmount); // 600.00m
        var reportMonth = DateTime.UtcNow;
        var pdfBytes = _pdfGeneratorService.GenerateTransactionReport(customer, orders, reportMonth);
        pdfBytes.Should().NotBeNull();
        expectedTotal.Should().Be(600.00m);
    }

    [Fact]
    public void GenerateTransactionReport_WithManyOrders_HandlesLargeDataSet()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "Bulk",
            LastName = "Buyer",
            Email = "bulk@example.com"
        };

        var orders = new List<Order>();
        for (int i = 1; i <= 100; i++)
        {
            orders.Add(new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = $"ORD-2024-{i:D4}",
                OrderDate = DateTime.UtcNow.AddDays(-i),
                TotalAmount = i * 10m
            });
        }

        var reportMonth = DateTime.UtcNow;
        var pdfBytes = _pdfGeneratorService.GenerateTransactionReport(customer, orders, reportMonth);
        pdfBytes.Should().NotBeNull();
        pdfBytes.Should().NotBeEmpty();
        pdfBytes.Length.Should().BeGreaterThan(5000, "Large PDF should have substantial size");

        var header = System.Text.Encoding.UTF8.GetString(pdfBytes.Take(4).ToArray());
        header.Should().Be("%PDF");
    }
}