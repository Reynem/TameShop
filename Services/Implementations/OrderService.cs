using Microsoft.EntityFrameworkCore;
using TameShop.Data;
using TameShop.Models;
using TameShop.Repositories.Interfaces;
using TameShop.Services.Interfaces;
using TameShop.ViewModels;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly TameShopDbContext _context;

    public OrderService(IOrderRepository orderRepository, TameShopDbContext context)
    {
        _orderRepository = orderRepository;
        _context = context;
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        return await _context.Orders.ToListAsync();
    }

    public async Task<List<Order>> GetOrdersByUserAsync(string userId)
    {
        return await _orderRepository.GetOrdersByUserIdAsync(userId);
    }

    public async Task<OrderDTO?> AddOrderItemAsync(int orderId, string userId, OrderItemDTO dto)
    {
        var order = await _orderRepository.GetOrderByIdAndUserIdAsync(orderId, userId);
        if (order == null) return null;
        if (!_orderRepository.CheckStatus(order)) throw new InvalidOperationException("Order not pending");

        var animal = await _context.Animals
            .Where(a => a.Id == dto.AnimalId)
            .Select(a => new { a.Name, a.Price })
            .FirstOrDefaultAsync();

        if (animal == null) throw new ArgumentException("Invalid Animal ID");

        var existingItem = order.Items.FirstOrDefault(i => i.AnimalId == dto.AnimalId);
        if (existingItem != null)
            existingItem.Quantity += dto.Quantity;
        else
            order.Items.Add(new OrderItem
            {
                AnimalId = dto.AnimalId,
                Price = animal.Price,
                AnimalName = animal.Name,
                Quantity = dto.Quantity
            });

        await _orderRepository.SaveChangesAsync();
        return OrderDTO.AutoMapper(order);
    }

    public async Task<OrderDTO?> DeleteOrderItemAsync(int orderId, string userId, OrderItemDTO dto)
    {
        var order = await _orderRepository.GetOrderByIdAndUserIdAsync(orderId, userId);
        if (order == null) return null;
        if (!_orderRepository.CheckStatus(order)) throw new InvalidOperationException("Order not pending");

        var item = order.Items.FirstOrDefault(i => i.AnimalId == dto.AnimalId);
        if (item == null) return null;

        if (item.Quantity > dto.Quantity)
            item.Quantity -= dto.Quantity;
        else
            order.Items.Remove(item);

        await _orderRepository.SaveChangesAsync();
        return OrderDTO.AutoMapper(order);
    }

    public async Task<OrderDTO?> CompleteOrderAsync(int orderId, string userId)
    {
        var order = await _orderRepository.GetOrderByIdAndUserIdAsync(orderId, userId);
        if (order == null) return null;
        if (!_orderRepository.CheckStatus(order)) throw new InvalidOperationException("Already confirmed");

        order.Status = OrderStatus.Completed;
        await _orderRepository.SaveChangesAsync();
        return OrderDTO.AutoMapper(order);
    }

    public async Task<OrderDTO?> CancelOrderAsync(int orderId, string userId)
    {
        var order = await _orderRepository.GetOrderByIdAndUserIdAsync(orderId, userId);
        if (order == null) return null;
        if (!_orderRepository.CheckStatus(order)) throw new InvalidOperationException("Already confirmed");

        order.Status = OrderStatus.Cancelled;
        await _orderRepository.SaveChangesAsync();
        return OrderDTO.AutoMapper(order);
    }
}
