using OnlineShopping.Core.Entities;

namespace OnlineShopping.Core.Interfaces;

/// <summary>
/// Order repository interface
/// </summary>
public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByOrderNumberAsync(string orderNumber);
    Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId);
    Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status);
    Task<Order?> GetWithItemsAsync(Guid orderId);
    Task<Order?> GetWithAllDetailsAsync(Guid orderId);
    Task<string> GenerateUniqueOrderNumberAsync();
}