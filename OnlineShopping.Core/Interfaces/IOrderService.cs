using OnlineShopping.Core.DTOs;
using OnlineShopping.Core.Entities;

namespace OnlineShopping.Core.Interfaces;

/// <summary>
/// Order service interface
/// </summary>
public interface IOrderService : IService
{
    Task<OrderDto> CreateOrderAsync(CreateOrderDto dto);
    Task<OrderDto?> GetOrderAsync(Guid orderId);
    Task<OrderDto?> GetOrderByNumberAsync(string orderNumber);
    Task<IEnumerable<OrderDto>> GetCustomerOrdersAsync(Guid customerId);
    Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(OrderStatus status);
    Task<OrderDto> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus);
    Task<bool> CancelOrderAsync(Guid orderId);
}