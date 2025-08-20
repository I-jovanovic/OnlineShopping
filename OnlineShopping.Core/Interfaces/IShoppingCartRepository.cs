using OnlineShopping.Core.Entities;

namespace OnlineShopping.Core.Interfaces;

/// <summary>
/// Shopping cart repository interface
/// </summary>
public interface IShoppingCartRepository : IRepository<ShoppingCart>
{
    Task<ShoppingCart?> GetByCustomerIdAsync(Guid customerId);
    Task<ShoppingCart?> GetWithItemsAsync(Guid cartId);
    Task<ShoppingCart?> GetActiveCartByCustomerIdAsync(Guid customerId);
    Task RemoveExpiredCartsAsync();
}