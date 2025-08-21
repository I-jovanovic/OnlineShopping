using Microsoft.Extensions.Logging;
using OnlineShopping.Core.Interfaces;
using OnlineShopping.Infrastructure.Resilience;
using Polly;

namespace OnlineShopping.Infrastructure.Services;

public class ResilientEmailService : IEmailService
{
    private readonly IEmailService _emailService;
    private readonly ILogger<ResilientEmailService> _logger;
    private readonly IAsyncPolicy _resiliencePolicy;

    public ResilientEmailService(
        EmailService emailService,
        ILogger<ResilientEmailService> logger)
    {
        _emailService = emailService;
        _logger = logger;
        _resiliencePolicy = ResiliencePolicies.GetCombinedPolicy(logger, nameof(EmailService));
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        await _resiliencePolicy.ExecuteAsync(async () =>
        {
            await _emailService.SendEmailAsync(to, subject, body, isHtml);
        });
    }

    public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachment, string attachmentName, bool isHtml = true)
    {
        await _resiliencePolicy.ExecuteAsync(async () =>
        {
            await _emailService.SendEmailWithAttachmentAsync(to, subject, body, attachment, attachmentName, isHtml);
        });
    }

    public async Task SendMonthlyTransactionReportAsync(Guid customerId, DateTime month)
    {
        await _resiliencePolicy.ExecuteAsync(async () =>
        {
            await _emailService.SendMonthlyTransactionReportAsync(customerId, month);
        });
    }

    public async Task SendBulkMonthlyReportsAsync(DateTime month)
    {
        await _resiliencePolicy.ExecuteAsync(async () =>
        {
            await _emailService.SendBulkMonthlyReportsAsync(month);
        });
    }
}