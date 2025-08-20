using AutoMapper;
using Microsoft.Extensions.Logging;
using OnlineShopping.Core.DTOs;
using OnlineShopping.Core.Entities;
using OnlineShopping.Core.Exceptions;
using OnlineShopping.Core.Interfaces;

namespace OnlineShopping.Infrastructure.Services;

/// <summary>
/// Order service implementation
/// </summary>
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IShoppingCartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepository,
        IShoppingCartRepository cartRepository,
        IProductRepository productRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto)
    {
        var customer = await _customerRepository.GetByIdAsync(dto.CustomerId);
        if (customer == null)
        {
            throw new NotFoundException($"Customer with ID {dto.CustomerId} not found");
        }

        var cart = await _cartRepository.GetWithItemsAsync(dto.CartId);
        if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
        {
            throw new BusinessRuleViolationException("Cart is empty or not found");
        }

        var orderNumber = await _orderRepository.GenerateUniqueOrderNumberAsync();
        
        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = orderNumber,
            CustomerId = dto.CustomerId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            ShippingAddressId = dto.ShippingAddressId,
            BillingAddressId = dto.BillingAddressId,
            PaymentMethod = dto.PaymentMethod,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            OrderItems = new List<OrderItem>()
        };

        decimal totalAmount = 0;

        foreach (var cartItem in cart.CartItems)
        {
            var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
            if (product == null)
            {
                throw new NotFoundException($"Product with ID {cartItem.ProductId} not found");
            }

            if (product.StockQuantity < cartItem.Quantity)
            {
                throw new InsufficientStockException(product.Name, cartItem.Quantity, product.StockQuantity);
            }

            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = product.Id,
                Quantity = cartItem.Quantity,
                UnitPrice = product.Price,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            order.OrderItems.Add(orderItem);
            totalAmount += orderItem.Quantity * orderItem.UnitPrice;

            product.StockQuantity -= cartItem.Quantity;
            await _productRepository.UpdateAsync(product);
        }

        order.TotalAmount = totalAmount;

        await _orderRepository.AddAsync(order);
        
        cart.CartItems.Clear();
        await _cartRepository.UpdateAsync(cart);
        
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Order created with ID: {OrderId}, Number: {OrderNumber}", order.Id, order.OrderNumber);

        return _mapper.Map<OrderDto>(order);
    }

    public async Task<OrderDto?> GetOrderAsync(Guid orderId)
    {
        var order = await _orderRepository.GetWithAllDetailsAsync(orderId);
        return order != null ? _mapper.Map<OrderDto>(order) : null;
    }

    public async Task<OrderDto?> GetOrderByNumberAsync(string orderNumber)
    {
        var order = await _orderRepository.GetByOrderNumberAsync(orderNumber);
        return order != null ? _mapper.Map<OrderDto>(order) : null;
    }

    public async Task<IEnumerable<OrderDto>> GetCustomerOrdersAsync(Guid customerId)
    {
        var orders = await _orderRepository.GetByCustomerIdAsync(customerId);
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(OrderStatus status)
    {
        var orders = await _orderRepository.GetByStatusAsync(status);
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<OrderDto> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            throw new NotFoundException($"Order with ID {orderId} not found");
        }

        var oldStatus = order.Status;
        order.Status = newStatus;
        order.UpdatedAt = DateTime.UtcNow;

        if (newStatus == OrderStatus.Shipped && oldStatus != OrderStatus.Shipped)
        {
            order.ShippedDate = DateTime.UtcNow;
        }
        else if (newStatus == OrderStatus.Delivered && oldStatus != OrderStatus.Delivered)
        {
            order.DeliveredDate = DateTime.UtcNow;
        }

        await _orderRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Order status updated. OrderId: {OrderId}, Status: {OldStatus} -> {NewStatus}", 
            orderId, oldStatus, newStatus);

        return _mapper.Map<OrderDto>(order);
    }

    public async Task<bool> CancelOrderAsync(Guid orderId)
    {
        var order = await _orderRepository.GetWithItemsAsync(orderId);
        if (order == null)
        {
            return false;
        }

        if (order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Delivered)
        {
            throw new BusinessRuleViolationException("Cannot cancel shipped or delivered orders");
        }

        foreach (var orderItem in order.OrderItems ?? Enumerable.Empty<OrderItem>())
        {
            var product = await _productRepository.GetByIdAsync(orderItem.ProductId);
            if (product != null)
            {
                product.StockQuantity += orderItem.Quantity;
                await _productRepository.UpdateAsync(product);
            }
        }

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;

        await _orderRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Order cancelled. OrderId: {OrderId}", orderId);

        return true;
    }

}