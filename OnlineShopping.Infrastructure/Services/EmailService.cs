using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using OnlineShopping.Core.Configuration;
using OnlineShopping.Core.Interfaces;

namespace OnlineShopping.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IPdfGeneratorService _pdfGenerator;

    public EmailService(
        IOptions<EmailSettings> emailSettings,
        ILogger<EmailService> logger,
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IPdfGeneratorService pdfGenerator)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _pdfGenerator = pdfGenerator;
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new BodyBuilder();
            if (isHtml)
                builder.HtmlBody = body;
            else
                builder.TextBody = body;

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, 
                _emailSettings.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls);
            
            await client.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }

    public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, 
        byte[] attachment, string attachmentName, bool isHtml = true)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new BodyBuilder();
            if (isHtml)
                builder.HtmlBody = body;
            else
                builder.TextBody = body;

            builder.Attachments.Add(attachmentName, attachment);
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort,
                _emailSettings.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls);
            
            await client.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email with attachment sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email with attachment to {To}", to);
            throw;
        }
    }

    public async Task SendMonthlyTransactionReportAsync(Guid customerId, DateTime month)
    {
        try
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                _logger.LogWarning("Customer {CustomerId} not found", customerId);
                return;
            }

            var startDate = new DateTime(month.Year, month.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            
            var orders = await _orderRepository.GetCustomerOrdersAsync(customerId, startDate, endDate);
            
            if (!orders.Any())
            {
                _logger.LogInformation("No orders found for customer {CustomerId} in {Month}", 
                    customerId, month.ToString("MMMM yyyy"));
                return;
            }

            var pdfReport = _pdfGenerator.GenerateTransactionReport(customer, orders.ToList(), month);
            
            var subject = $"Your Transaction Report for {month:MMMM yyyy}";
            var body = GenerateEmailBody(customer, orders.ToList(), month);
            var attachmentName = $"TransactionReport_{month:yyyy-MM}.pdf";

            await SendEmailWithAttachmentAsync(customer.Email, subject, body, pdfReport, attachmentName);
            
            _logger.LogInformation("Monthly report sent to customer {CustomerId}", customerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send monthly report to customer {CustomerId}", customerId);
            throw;
        }
    }

    public async Task SendBulkMonthlyReportsAsync(DateTime month)
    {
        try
        {
            var customers = await _customerRepository.GetActiveCustomersAsync();
            var tasks = new List<Task>();

            foreach (var customer in customers)
            {
                tasks.Add(SendMonthlyTransactionReportAsync(customer.Id, month));
            }

            await Task.WhenAll(tasks);
            _logger.LogInformation("Bulk monthly reports sent for {Month}", month.ToString("MMMM yyyy"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send bulk monthly reports");
            throw;
        }
    }

    private string GenerateEmailBody(Core.Entities.Customer customer, List<Core.Entities.Order> orders, DateTime month)
    {
        var totalAmount = orders.Sum(o => o.TotalAmount);
        var orderCount = orders.Count;

        return $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Dear {customer.FirstName} {customer.LastName},</h2>
                <p>Please find attached your transaction report for {month:MMMM yyyy}.</p>
                
                <h3>Summary:</h3>
                <ul>
                    <li>Total Orders: {orderCount}</li>
                    <li>Total Amount: ${totalAmount:F2}</li>
                </ul>
                
                <p>Thank you for your continued business!</p>
                
                <p>Best regards,<br/>
                Online Shopping Team</p>
            </body>
            </html>";
    }
}