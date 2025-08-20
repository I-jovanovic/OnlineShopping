using OnlineShopping.Core.Entities;

namespace OnlineShopping.Core.Interfaces;

/// <summary>
/// Payment repository interface
/// </summary>
public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetByOrderIdAsync(Guid orderId);
    Task<Payment?> GetByTransactionIdAsync(string transactionId);
    Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status);
    Task<IEnumerable<Payment>> GetPendingPaymentsAsync(TimeSpan olderThan);
}