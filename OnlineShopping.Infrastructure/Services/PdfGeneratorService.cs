using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using OnlineShopping.Core.Entities;
using OnlineShopping.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace OnlineShopping.Infrastructure.Services;

public class PdfGeneratorService : IPdfGeneratorService
{
    private readonly ILogger<PdfGeneratorService> _logger;

    public PdfGeneratorService(ILogger<PdfGeneratorService> logger)
    {
        _logger = logger;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateTransactionReport(Customer customer, List<Order> orders, DateTime month)
    {
        try
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    
                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(10);
                            
                            column.Item().Text($"Transaction Report - {month:MMMM yyyy}")
                                .FontSize(16).Bold();
                            
                            column.Item().Text($"Customer: {customer.FirstName} {customer.LastName}");
                            column.Item().Text($"Email: {customer.Email}");
                            
                            column.Item().PaddingVertical(10);
                            
                            column.Item().Text("Orders:").Bold();
                            foreach (var order in orders)
                            {
                                column.Item().Text($"Order #{order.OrderNumber} - {order.OrderDate:yyyy-MM-dd} - ${order.TotalAmount:F2}");
                            }
                            
                            column.Item().PaddingVertical(10);

                            column.Item().Text($"Total Amount: ${orders.Sum(o => o.TotalAmount):F2}").Bold();
                        });
                });
            });

            return document.GeneratePdf();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate transaction report PDF");
            throw;
        }
    }
}