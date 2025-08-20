using Microsoft.EntityFrameworkCore;
using OnlineShopping.Core.Entities;
using OnlineShopping.Core.Interfaces;

namespace OnlineShopping.Infrastructure.Persistence.Repositories;

/// <summary>
/// Payment repository implementation
/// </summary>
public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Payment?> GetByOrderIdAsync(Guid orderId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.OrderId == orderId);
    }

    public async Task<Payment?> GetByTransactionIdAsync(string transactionId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.TransactionId == transactionId);
    }

    public async Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status)
    {
        return await _dbSet
            .Where(p => p.Status == status)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetPendingPaymentsAsync(TimeSpan olderThan)
    {
        var cutoffDate = DateTime.UtcNow.Subtract(olderThan);
        return await _dbSet
            .Where(p => p.Status == PaymentStatus.Pending && p.PaymentDate < cutoffDate)
            .OrderBy(p => p.PaymentDate)
            .ToListAsync();
    }
}