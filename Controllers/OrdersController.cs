using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TameShop.Data;
using TameShop.Models;
using TameShop.ViewModels;

namespace TameShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class OrdersController : ControllerBase
    {
        private readonly TameShopDbContext _context;

        public OrdersController(TameShopDbContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByUser()
        {
            var orders = await GetOrdersByUserIdAsync();

            if (orders == null)
            {
                return NotFound();
            }

            return orders;
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutOrder(int id, Order order)
        //{
        //    if (id != order.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(order).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!OrderExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        [HttpPost("{id}")]
        public async Task<ActionResult<OrderDTO>> PostOrderItem(int id, [FromBody] OrderItemDTO orderItemDTO)
        {
            var userId = GetValidUserAsync();

            // Creation of a new order will be handled in other methods, so I assume the order already exists
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                return NotFound("Order not found.");
            }

            // Order status check
            if (order.Status != OrderStatus.Pending)
            {
                return BadRequest("Cannot modify order after it's confirmed.");
            }

            // Get the animal name and price
            var animal = await _context.Animals
                .Where(a => a.Id == orderItemDTO.AnimalId)
                .Select(a => new { a.Name, a.Price })
                .FirstOrDefaultAsync();

            if (animal == null)
            {
                return BadRequest("Invalid Animal ID.");
            }

            // Check if the order item already exists
            var existingItem = order.Items.FirstOrDefault(i => i.AnimalId == orderItemDTO.AnimalId);
            if (existingItem != null)
            {
                existingItem.Quantity += orderItemDTO.Quantity;
            }
            else
            {
                var orderItem = new OrderItem
                {
                    AnimalId = orderItemDTO.AnimalId,
                    Price = animal.Price,
                    AnimalName = animal.Name,
                    Quantity = orderItemDTO.Quantity
                };

                order.Items.Add(orderItem);
            }

            await _context.SaveChangesAsync();

            var orderDTO = OrderDTO.AutoMapper(order);
            return Ok(orderDTO);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItem(int id, [FromBody] OrderItemDTO orderItemDTO)
        {
            var userId = GetValidUserAsync();

            // Find the order
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                return NotFound("Order not found.");
            }

            // Order status check
            if (order.Status != OrderStatus.Pending)
            {
                return BadRequest("Cannot modify order after it's confirmed.");
            }

            // Find the item to delete
            var itemToDelete = order.Items.FirstOrDefault(i => i.AnimalId == orderItemDTO.AnimalId);
            if (itemToDelete == null)
            {
                return NotFound("Order item not found.");
            }

            // Remove the item
            order.Items.Remove(itemToDelete);
            await _context.SaveChangesAsync();
            var orderDTO = OrderDTO.AutoMapper(order);
            return Ok(orderDTO);
        }

        // DELETE: api/Orders/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteOrder(int id)
        //{
        //    var order = await _context.Orders.FindAsync(id);
        //    if (order == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Orders.Remove(order);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

        private string GetValidUserAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException();
        }

        private async Task<List<Order>> GetOrdersByUserIdAsync()
        {
            try
            {
                var userId = GetValidUserAsync();

                var orders = await _context.Orders
                    .Include(c => c.Items)
                    .Where(o => o.UserId == userId)
                    .ToListAsync();

                return orders;
            }
            catch (UnauthorizedAccessException e)
            {
                throw new UnauthorizedAccessException("An error occurred while retrieving orders.", e);
            }
        }
    }
}
