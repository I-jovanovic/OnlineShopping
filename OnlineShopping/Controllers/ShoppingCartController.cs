using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Core.DTOs;
using OnlineShopping.Core.Exceptions;
using OnlineShopping.Core.Interfaces;

namespace OnlineShopping.Controllers;

/// <summary>
/// Shopping cart management endpoints
/// </summary>
public class ShoppingCartController : ApiControllerBase
{
    private readonly IShoppingCartService _cartService;
    private readonly ILogger<ShoppingCartController> _logger;

    public ShoppingCartController(
        IShoppingCartService cartService,
        ILogger<ShoppingCartController> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }

    /// <summary>
    /// Get or create cart for customer
    /// </summary>
    [HttpGet("customer/{customerId:guid}")]
    [ProducesResponseType(typeof(ShoppingCartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShoppingCartDto>> GetOrCreateCart(Guid customerId)
    {
        try
        {
            var cart = await _cartService.GetOrCreateCartAsync(customerId);
            return Ok(cart);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Customer not found");
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get cart by ID
    /// </summary>
    [HttpGet("{cartId:guid}")]
    [ProducesResponseType(typeof(ShoppingCartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShoppingCartDto>> GetCart(Guid cartId)
    {
        var cart = await _cartService.GetCartAsync(cartId);
        if (cart == null)
        {
            return NotFound();
        }
        return Ok(cart);
    }

    /// <summary>
    /// Add item to cart
    /// </summary>
    [HttpPost("{customerId:guid}/items")]
    [ProducesResponseType(typeof(ShoppingCartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShoppingCartDto>> AddItemToCart(Guid customerId, [FromBody] AddToCartDto dto)
    {
        try
        {
            var cart = await _cartService.AddItemToCartAsync(customerId, dto);
            return Ok(cart);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Product or customer not found");
            return NotFound(new { error = ex.Message });
        }
        catch (BusinessRuleViolationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation adding item to cart");
            return BadRequest(new { error = ex.Message });
        }
        catch (InsufficientStockException ex)
        {
            _logger.LogWarning(ex, "Insufficient stock");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Update cart item quantity
    /// </summary>
    [HttpPut("{cartId:guid}/items/{itemId:guid}")]
    [ProducesResponseType(typeof(ShoppingCartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShoppingCartDto>> UpdateCartItem(Guid cartId, Guid itemId, [FromBody] int quantity)
    {
        try
        {
            var cart = await _cartService.UpdateCartItemAsync(cartId, itemId, quantity);
            return Ok(cart);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Cart or item not found");
            return NotFound(new { error = ex.Message });
        }
        catch (InsufficientStockException ex)
        {
            _logger.LogWarning(ex, "Insufficient stock");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Remove item from cart
    /// </summary>
    [HttpDelete("{cartId:guid}/items/{itemId:guid}")]
    [ProducesResponseType(typeof(ShoppingCartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShoppingCartDto>> RemoveItemFromCart(Guid cartId, Guid itemId)
    {
        try
        {
            var cart = await _cartService.RemoveItemFromCartAsync(cartId, itemId);
            return Ok(cart);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Cart or item not found");
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Clear cart
    /// </summary>
    [HttpDelete("{cartId:guid}/clear")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClearCart(Guid cartId)
    {
        var cleared = await _cartService.ClearCartAsync(cartId);
        if (!cleared)
        {
            return NotFound();
        }
        return NoContent();
    }

    /// <summary>
    /// Remove expired carts (admin endpoint)
    /// </summary>
    [HttpPost("cleanup")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveExpiredCarts()
    {
        await _cartService.RemoveExpiredCartsAsync();
        return NoContent();
    }
}