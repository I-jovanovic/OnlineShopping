using OnlineShopping.Core.Entities;

namespace OnlineShopping.Core.Interfaces;

/// <summary>
/// Payment service interface
/// </summary>
public interface IPaymentService : IService
{
    Task<Payment> ProcessPaymentAsync(Guid orderId, string paymentMethod, decimal amount);
    Task<Payment?> GetPaymentByIdAsync(Guid id);
    Task<IEnumerable<Payment>> GetOrderPaymentsAsync(Guid orderId);
    Task<Payment> UpdatePaymentStatusAsync(Guid paymentId, PaymentStatus status);
    Task<Payment> RefundPaymentAsync(Guid paymentId, decimal amount);
    Task<bool> IsPaymentCompleteForOrderAsync(Guid orderId);
    Task<decimal> GetTotalPaidAmountAsync(Guid orderId);
}