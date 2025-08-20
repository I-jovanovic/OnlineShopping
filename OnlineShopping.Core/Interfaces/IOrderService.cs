using OnlineShopping.Core.Entities;

namespace OnlineShopping.Core.Interfaces;

/// <summary>
/// Order service interface
/// </summary>
public interface IOrderService : IService
{
    Task<Order> CreateOrderFromCartAsync(Guid customerId, Guid shippingAddressId, Guid billingAddressId);
    Task<Order> CreateOrderAsync(Guid customerId, IEnumerable<CartItem> items, Guid shippingAddressId, Guid billingAddressId);
    Task<Order?> GetOrderByIdAsync(Guid id);
    Task<Order?> GetOrderByOrderNumberAsync(string orderNumber);
    Task<IEnumerable<Order>> GetCustomerOrdersAsync(Guid customerId);
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task<Order> UpdateOrderStatusAsync(Guid orderId, OrderStatus status);
    Task CancelOrderAsync(Guid orderId);
    Task<decimal> CalculateOrderTotalAsync(IEnumerable<CartItem> items);
    Task<string> GenerateOrderNumberAsync();
    Task<bool> CanCancelOrderAsync(Guid orderId);
}