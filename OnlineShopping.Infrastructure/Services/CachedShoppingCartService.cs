using Microsoft.Extensions.Logging;
using OnlineShopping.Core.DTOs;
using OnlineShopping.Core.Interfaces;

namespace OnlineShopping.Infrastructure.Services;

public class CachedShoppingCartService : IShoppingCartService
{
    private readonly IShoppingCartService _shoppingCartService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachedShoppingCartService> _logger;
    private const string CartCacheKeyPrefix = "cart:";

    public CachedShoppingCartService(
        ShoppingCartService shoppingCartService,
        ICacheService cacheService,
        ILogger<CachedShoppingCartService> logger)
    {
        _shoppingCartService = shoppingCartService;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ShoppingCartDto> GetOrCreateCartAsync(Guid customerId)
    {
        var cacheKey = $"cart:customer:{customerId}";
        
        var cachedCart = await _cacheService.GetAsync<ShoppingCartDto>(cacheKey);
        if (cachedCart != null)
        {
            return cachedCart;
        }

        var cart = await _shoppingCartService.GetOrCreateCartAsync(customerId);
        
        await _cacheService.SetAsync(cacheKey, cart, TimeSpan.FromMinutes(15));
        await _cacheService.SetAsync($"{CartCacheKeyPrefix}{cart.Id}", cart, TimeSpan.FromMinutes(15));
        
        return cart;
    }

    public async Task<ShoppingCartDto?> GetCartAsync(Guid cartId)
    {
        var cacheKey = $"{CartCacheKeyPrefix}{cartId}";
        
        var cachedCart = await _cacheService.GetAsync<ShoppingCartDto>(cacheKey);
        if (cachedCart != null)
        {
            return cachedCart;
        }

        var cart = await _shoppingCartService.GetCartAsync(cartId);
        if (cart != null)
        {
            await _cacheService.SetAsync(cacheKey, cart, TimeSpan.FromMinutes(15));
        }

        return cart;
    }


    public async Task<ShoppingCartDto> AddItemToCartAsync(Guid customerId, AddToCartDto dto)
    {
        var result = await _shoppingCartService.AddItemToCartAsync(customerId, dto);
        
        await InvalidateCartCaches(result.Id, result.CustomerId);
        
        await _cacheService.SetAsync($"{CartCacheKeyPrefix}{result.Id}", result, TimeSpan.FromMinutes(15));
        
        return result;
    }

    public async Task<ShoppingCartDto> UpdateCartItemAsync(Guid cartId, Guid itemId, int quantity)
    {
        var result = await _shoppingCartService.UpdateCartItemAsync(cartId, itemId, quantity);
        
        await InvalidateCartCaches(cartId, result.CustomerId);
        
        await _cacheService.SetAsync($"{CartCacheKeyPrefix}{cartId}", result, TimeSpan.FromMinutes(15));
        
        return result;
    }

    public async Task<ShoppingCartDto> RemoveItemFromCartAsync(Guid cartId, Guid itemId)
    {
        var result = await _shoppingCartService.RemoveItemFromCartAsync(cartId, itemId);
        
        await InvalidateCartCaches(cartId, result.CustomerId);
        
        await _cacheService.SetAsync($"{CartCacheKeyPrefix}{cartId}", result, TimeSpan.FromMinutes(15));
        
        return result;
    }

    public async Task<bool> ClearCartAsync(Guid cartId)
    {
        var cart = await GetCartAsync(cartId);
        
        var result = await _shoppingCartService.ClearCartAsync(cartId);
        
        if (result && cart != null)
        {
            await InvalidateCartCaches(cartId, cart.CustomerId);
        }

        return result;
    }

    public async Task RemoveExpiredCartsAsync()
    {
        await _shoppingCartService.RemoveExpiredCartsAsync();
        
        await _cacheService.RemoveByPatternAsync("cart:*");
    }

    private async Task InvalidateCartCaches(Guid cartId, Guid customerId)
    {
        await _cacheService.RemoveAsync($"{CartCacheKeyPrefix}{cartId}");
        await _cacheService.RemoveAsync($"cart:customer:{customerId}");
    }
}