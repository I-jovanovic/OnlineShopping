using Hangfire;
using Microsoft.Extensions.Logging;
using OnlineShopping.Core.Interfaces;

namespace OnlineShopping.Infrastructure.Services;

public class BackgroundJobService : IBackgroundJobService
{
    private readonly ILogger<BackgroundJobService> _logger;
    private readonly IEmailService _emailService;
    private readonly IRecurringJobManager _recurringJobManager;

    public BackgroundJobService(
        ILogger<BackgroundJobService> logger,
        IEmailService emailService,
        IRecurringJobManager recurringJobManager)
    {
        _logger = logger;
        _emailService = emailService;
        _recurringJobManager = recurringJobManager;
    }

    public void ScheduleMonthlyReports()
    {
        // Schedule to run on the 1st of every month at 2 AM
        _recurringJobManager.AddOrUpdate(
            "monthly-transaction-reports",
            () => SendMonthlyReportsJob(),
            "0 2 1 * *");

        _logger.LogInformation("Monthly transaction reports scheduled");
    }

    public async Task SendMonthlyReportsJob()
    {
        try
        {
            _logger.LogInformation("Starting monthly transaction reports job");
            
            var previousMonth = DateTime.UtcNow.AddMonths(-1);
            await _emailService.SendBulkMonthlyReportsAsync(previousMonth);
            
            _logger.LogInformation("Monthly transaction reports job completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute monthly reports job");
            throw;
        }
    }
}