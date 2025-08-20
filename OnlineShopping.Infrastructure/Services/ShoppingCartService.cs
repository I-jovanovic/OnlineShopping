using AutoMapper;
using Microsoft.Extensions.Logging;
using OnlineShopping.Core.DTOs;
using OnlineShopping.Core.Entities;
using OnlineShopping.Core.Exceptions;
using OnlineShopping.Core.Interfaces;

namespace OnlineShopping.Infrastructure.Services;

/// <summary>
/// Shopping cart service implementation
/// </summary>
public class ShoppingCartService : IShoppingCartService
{
    private readonly IShoppingCartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ShoppingCartService> _logger;

    public ShoppingCartService(
        IShoppingCartRepository cartRepository,
        IProductRepository productRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ShoppingCartService> logger)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ShoppingCartDto> GetOrCreateCartAsync(Guid customerId)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null)
        {
            throw new NotFoundException($"Customer with ID {customerId} not found");
        }

        var cart = await _cartRepository.GetActiveCartByCustomerIdAsync(customerId);
        
        if (cart == null)
        {
            cart = new ShoppingCart
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CartItems = new List<CartItem>()
            };

            await _cartRepository.AddAsync(cart);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Shopping cart created for customer: {CustomerId}", customerId);
        }

        return _mapper.Map<ShoppingCartDto>(cart);
    }

    public async Task<ShoppingCartDto?> GetCartAsync(Guid cartId)
    {
        var cart = await _cartRepository.GetWithItemsAsync(cartId);
        return cart != null ? _mapper.Map<ShoppingCartDto>(cart) : null;
    }

    public async Task<ShoppingCartDto> AddItemToCartAsync(Guid customerId, AddToCartDto dto)
    {
        var cart = await _cartRepository.GetActiveCartByCustomerIdAsync(customerId);
        if (cart == null)
        {
            cart = new ShoppingCart
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CartItems = new List<CartItem>()
            };
            await _cartRepository.AddAsync(cart);
        }

        var product = await _productRepository.GetByIdAsync(dto.ProductId);
        if (product == null)
        {
            throw new NotFoundException($"Product with ID {dto.ProductId} not found");
        }

        if (!product.IsActive)
        {
            throw new BusinessRuleViolationException("Product is not available");
        }

        if (product.StockQuantity < dto.Quantity)
        {
            throw new InsufficientStockException(product.Name, dto.Quantity, product.StockQuantity);
        }

        var existingItem = cart.CartItems?.FirstOrDefault(ci => ci.ProductId == dto.ProductId);
        
        if (existingItem != null)
        {
            existingItem.Quantity += dto.Quantity;
            existingItem.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            var cartItem = new CartItem
            {
                Id = Guid.NewGuid(),
                ShoppingCartId = cart.Id,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                Price = product.Price,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (cart.CartItems == null)
                cart.CartItems = new List<CartItem>();
            
            cart.CartItems.Add(cartItem);
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Item added to cart. CartId: {CartId}, ProductId: {ProductId}", cart.Id, dto.ProductId);

        cart = await _cartRepository.GetWithItemsAsync(cart.Id);
        return _mapper.Map<ShoppingCartDto>(cart!);
    }

    public async Task<ShoppingCartDto> UpdateCartItemAsync(Guid cartId, Guid cartItemId, int quantity)
    {
        var cart = await _cartRepository.GetWithItemsAsync(cartId);
        if (cart == null)
        {
            throw new NotFoundException($"Cart with ID {cartId} not found");
        }

        var cartItem = cart.CartItems?.FirstOrDefault(ci => ci.Id == cartItemId);
        if (cartItem == null)
        {
            throw new NotFoundException($"Cart item with ID {cartItemId} not found");
        }

        if (quantity <= 0)
        {
            cart.CartItems!.Remove(cartItem);
        }
        else
        {
            var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
            if (product == null || product.StockQuantity < quantity)
            {
                throw new InsufficientStockException(product?.Name ?? "Product", quantity, product?.StockQuantity ?? 0);
            }

            cartItem.Quantity = quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Cart item updated. CartId: {CartId}, ItemId: {ItemId}", cartId, cartItemId);

        return _mapper.Map<ShoppingCartDto>(cart);
    }

    public async Task<ShoppingCartDto> RemoveItemFromCartAsync(Guid cartId, Guid cartItemId)
    {
        var cart = await _cartRepository.GetWithItemsAsync(cartId);
        if (cart == null)
        {
            throw new NotFoundException($"Cart with ID {cartId} not found");
        }

        var cartItem = cart.CartItems?.FirstOrDefault(ci => ci.Id == cartItemId);
        if (cartItem == null)
        {
            throw new NotFoundException($"Cart item with ID {cartItemId} not found");
        }

        cart.CartItems!.Remove(cartItem);
        cart.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Item removed from cart. CartId: {CartId}, ItemId: {ItemId}", cartId, cartItemId);

        return _mapper.Map<ShoppingCartDto>(cart);
    }

    public async Task<bool> ClearCartAsync(Guid cartId)
    {
        var cart = await _cartRepository.GetWithItemsAsync(cartId);
        if (cart == null)
        {
            return false;
        }

        cart.CartItems?.Clear();
        cart.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Cart cleared. CartId: {CartId}", cartId);

        return true;
    }

    public async Task RemoveExpiredCartsAsync()
    {
        await _cartRepository.RemoveExpiredCartsAsync();
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Expired carts removed");
    }

}