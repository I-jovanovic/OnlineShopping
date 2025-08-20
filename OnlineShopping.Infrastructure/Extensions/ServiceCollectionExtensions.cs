using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineShopping.Core.Interfaces;
using OnlineShopping.Infrastructure.Persistence;
using OnlineShopping.Infrastructure.Persistence.Repositories;
using OnlineShopping.Infrastructure.Services;

namespace OnlineShopping.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring infrastructure services
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Add AutoMapper
        services.AddAutoMapper(typeof(ServiceCollectionExtensions).Assembly);

        // Add generic repository and unit of work
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Add specific repositories
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        // Add business services
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IShoppingCartService, ShoppingCartService>();
        services.AddScoped<ICategoryService, CategoryService>();

        return services;
    }
}