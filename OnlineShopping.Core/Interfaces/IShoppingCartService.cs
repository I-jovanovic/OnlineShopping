using OnlineShopping.Core.Entities;

namespace OnlineShopping.Core.Interfaces;

/// <summary>
/// Shopping cart service interface
/// </summary>
public interface IShoppingCartService : IService
{
    Task<ShoppingCart> GetOrCreateCartAsync(Guid customerId);
    Task<ShoppingCart?> GetCartByIdAsync(Guid cartId);
    Task<CartItem> AddItemAsync(Guid customerId, Guid productId, int quantity);
    Task<CartItem> UpdateItemQuantityAsync(Guid cartItemId, int quantity);
    Task RemoveItemAsync(Guid cartItemId);
    Task ClearCartAsync(Guid customerId);
    Task<decimal> GetCartTotalAsync(Guid customerId);
    Task<int> GetCartItemCountAsync(Guid customerId);
    Task<bool> IsItemInCartAsync(Guid customerId, Guid productId);
}