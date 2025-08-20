using OnlineShopping.Core.DTOs;

namespace OnlineShopping.Core.Interfaces;

/// <summary>
/// Shopping cart service interface
/// </summary>
public interface IShoppingCartService : IService
{
    Task<ShoppingCartDto> GetOrCreateCartAsync(Guid customerId);
    Task<ShoppingCartDto?> GetCartAsync(Guid cartId);
    Task<ShoppingCartDto> AddItemToCartAsync(Guid customerId, AddToCartDto dto);
    Task<ShoppingCartDto> UpdateCartItemAsync(Guid cartId, Guid cartItemId, int quantity);
    Task<ShoppingCartDto> RemoveItemFromCartAsync(Guid cartId, Guid cartItemId);
    Task<bool> ClearCartAsync(Guid cartId);
    Task RemoveExpiredCartsAsync();
}