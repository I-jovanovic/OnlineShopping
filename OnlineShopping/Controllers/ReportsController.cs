using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Core.Interfaces;

namespace OnlineShopping.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly IBackgroundJobService _backgroundJobService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(
        IEmailService emailService,
        IBackgroundJobService backgroundJobService,
        ILogger<ReportsController> logger)
    {
        _emailService = emailService;
        _backgroundJobService = backgroundJobService;
        _logger = logger;
    }

    /// <summary>
    /// Send monthly transaction report to a specific customer
    /// </summary>
    [HttpPost("monthly/{customerId}")]
    public async Task<IActionResult> SendMonthlyReport(Guid customerId, [FromQuery] DateTime? month = null)
    {
        try
        {
            var reportMonth = month ?? DateTime.UtcNow.AddMonths(-1);
            await _emailService.SendMonthlyTransactionReportAsync(customerId, reportMonth);
            
            return Ok(new { message = $"Monthly report sent successfully for {reportMonth:MMMM yyyy}" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send monthly report for customer {CustomerId}", customerId);
            return StatusCode(500, new { error = "Failed to send monthly report" });
        }
    }

    /// <summary>
    /// Manually trigger monthly reports for all customers
    /// </summary>
    [HttpPost("trigger-monthly")]
    public async Task<IActionResult> TriggerMonthlyReports()
    {
        try
        {
            await _backgroundJobService.SendMonthlyReportsJob();
            return Ok(new { message = "Monthly reports triggered successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to trigger monthly reports");
            return StatusCode(500, new { error = "Failed to trigger monthly reports" });
        }
    }
}