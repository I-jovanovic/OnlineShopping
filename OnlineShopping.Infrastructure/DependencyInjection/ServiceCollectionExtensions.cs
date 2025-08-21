using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OnlineShopping.Core.Configuration;
using OnlineShopping.Core.Interfaces;
using OnlineShopping.Infrastructure.Http;
using OnlineShopping.Infrastructure.Persistence;
using OnlineShopping.Infrastructure.Persistence.Repositories;
using OnlineShopping.Infrastructure.Services;

namespace OnlineShopping.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
            });
        });

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddScoped<ProductService>();
        services.AddScoped<IProductService>(provider =>
        {
            var productService = provider.GetRequiredService<ProductService>();
            var cacheService = provider.GetRequiredService<ICacheService>();
            var logger = provider.GetRequiredService<ILogger<CachedProductService>>();
            return new CachedProductService(productService, cacheService, logger);
        });
        
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IOrderService, OrderService>();
        
        services.AddScoped<ShoppingCartService>();
        services.AddScoped<IShoppingCartService>(provider =>
        {
            var cartService = provider.GetRequiredService<ShoppingCartService>();
            var cacheService = provider.GetRequiredService<ICacheService>();
            var logger = provider.GetRequiredService<ILogger<CachedShoppingCartService>>();
            return new CachedShoppingCartService(cartService, cacheService, logger);
        });
        
        services.AddScoped<ICategoryService, CategoryService>();
        
        // Cache service
        services.AddSingleton<ICacheService, DistributedCacheService>();
        
        // Email and PDF services with resilience
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddScoped<EmailService>();
        services.AddScoped<IEmailService>(provider =>
        {
            var emailService = provider.GetRequiredService<EmailService>();
            var logger = provider.GetRequiredService<ILogger<ResilientEmailService>>();
            return new ResilientEmailService(emailService, logger);
        });
        services.AddScoped<IPdfGeneratorService, PdfGeneratorService>();
        
        // HTTP client with resilience
        services.AddHttpClient<IResilientHttpClient, ResilientHttpClient>();
        
        // Background job service
        services.AddScoped<IBackgroundJobService, BackgroundJobService>();
        
        // Hangfire
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetSection("Hangfire:ConnectionString").Value ?? 
                configuration.GetConnectionString("DefaultConnection"), 
                new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

        services.AddHangfireServer();

        return services;
    }
}