using OnlineShopping.Core.Entities;

namespace OnlineShopping.Core.Interfaces;

public interface IPdfGeneratorService
{
    byte[] GenerateTransactionReport(Customer customer, List<Order> orders, DateTime month);
}