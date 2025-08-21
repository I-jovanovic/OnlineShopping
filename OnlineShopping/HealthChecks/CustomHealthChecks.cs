using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace OnlineShopping.HealthChecks;

public class EmailServiceHealthCheck : IHealthCheck
{
    private readonly ILogger<EmailServiceHealthCheck> _logger;

    public EmailServiceHealthCheck(ILogger<EmailServiceHealthCheck> logger)
    {
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var isHealthy = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("Email__SmtpServer"));
            
            return Task.FromResult(isHealthy
                ? HealthCheckResult.Healthy("Email service is configured")
                : HealthCheckResult.Unhealthy("Email service is not configured"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email service health check failed");
            return Task.FromResult(HealthCheckResult.Unhealthy("Email service check failed", ex));
        }
    }
}

public class HangfireHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__HangfireConnection");
            var isHealthy = !string.IsNullOrEmpty(connectionString);
            
            return Task.FromResult(isHealthy
                ? HealthCheckResult.Healthy("Hangfire is configured")
                : HealthCheckResult.Degraded("Hangfire connection not configured"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Hangfire check failed", ex));
        }
    }
}

public class BusinessLogicHealthCheck : IHealthCheck
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BusinessLogicHealthCheck> _logger;

    public BusinessLogicHealthCheck(
        IServiceProvider serviceProvider,
        ILogger<BusinessLogicHealthCheck> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            
            var productService = scope.ServiceProvider.GetService<Core.Interfaces.IProductService>();
            if (productService == null)
            {
                return HealthCheckResult.Unhealthy("Product service not available");
            }

            var products = await productService.GetAllProductsAsync();
            
            return HealthCheckResult.Healthy($"Business logic healthy. Products count: {products.Count()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Business logic health check failed");
            return HealthCheckResult.Unhealthy("Business logic check failed", ex);
        }
    }
}