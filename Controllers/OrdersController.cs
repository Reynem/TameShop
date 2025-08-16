using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TameShop.Models;
using TameShop.Services.Interfaces;
using TameShop.ViewModels;

namespace TameShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByUser()
        {
            var userId = GetValidUserId();
            var orders = await _orderService.GetOrdersByUserAsync(userId);

            if (orders == null || !orders.Any())
                return NotFound();

            return Ok(orders);
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<OrderDTO>> PostOrderItem(int id, [FromBody] OrderItemDTO dto)
        {
            var userId = GetValidUserId();
            try
            {
                var result = await _orderService.AddOrderItemAsync(id, userId, dto);
                if (result == null) return NotFound("Order not found.");
                return Ok(result);
            }
            catch (ArgumentException e) { return BadRequest(e.Message); }
            catch (InvalidOperationException e) { return BadRequest(e.Message); }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<OrderDTO>> DeleteOrderItem(int id, [FromBody] OrderItemDTO dto)
        {
            var userId = GetValidUserId();
            try
            {
                var result = await _orderService.DeleteOrderItemAsync(id, userId, dto);
                if (result == null) return NotFound("Order or item not found.");
                return Ok(result);
            }
            catch (InvalidOperationException e) { return BadRequest(e.Message); }
        }

        [HttpPut("{id}/confirm")]
        public async Task<ActionResult<OrderDTO>> CompleteOrder(int id)
        {
            var userId = GetValidUserId();
            try
            {
                var result = await _orderService.CompleteOrderAsync(id, userId);
                if (result == null) return NotFound("Order not found.");
                return Ok(result);
            }
            catch (InvalidOperationException e) { return BadRequest(e.Message); }
        }

        [HttpPatch("{id}/cancel")]
        public async Task<ActionResult<OrderDTO>> CancelOrder(int id)
        {
            var userId = GetValidUserId();
            try
            {
                var result = await _orderService.CancelOrderAsync(id, userId);
                if (result == null) return NotFound("Order not found.");
                return Ok(result);
            }
            catch (InvalidOperationException e) { return BadRequest(e.Message); }
        }

        private string GetValidUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated.");
            return userId;
        }
    }
}
