using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaPlusPlus.Data;
using PharmaPlusPlus.Models;

namespace PharmaPlusPlus.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    public class OrderController : ControllerBase
    {
        private readonly PharmaPlusPlusContext _context;

        public OrderController(PharmaPlusPlusContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            List<Order> orders = new();
            if (HttpContext.User.IsInRole("Admin"))
            {
                orders = await _context.Orders.ToListAsync();
            }
            else
            {
                var userId = Guid.Parse(HttpContext.User.Identity.Name);
                orders = await _context.Orders.Where(c => c.UserId == userId).ToListAsync();
            }

            if (orders.Count == 0)
            {
                return NotFound("No Orders found. DB is empty.");
            }

            return Ok(orders);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("getOrdersByUserId/{userId:Guid}")]
        public async Task<IActionResult> GetOrdersByUserId(Guid userId)
        {
            // A safe check to ensure that the user exists in the database
            var user = await _context.Users.FindAsync(userId);
            if (user is null)
            {
                return NotFound("User doesn't exist with the given userId.");
            }

            List<Order> orderItems = await _context.Orders.Where(c => c.UserId == userId).ToListAsync();
            if (orderItems.Count == 0)
            {
                return NotFound("No Orders found for the given userId");
            }

            return Ok(orderItems);
        }

        [HttpGet("getOrderByOrderId/{orderId:Guid}")]
        public async Task<IActionResult> GetOrderByOrderId(Guid orderId)
        {
            // If the user is an Admin, they can view all the orders
            if (HttpContext.User.IsInRole("Admin"))
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order is null)
                {
                    return NotFound("Order not found with the given order id");
                }

                return Ok(order);
            }
            // If the user is not an Admin, they can only view their orders
            else
            {
                var userId = Guid.Parse(HttpContext.User.Identity.Name);
                var order = await _context.Orders.FirstOrDefaultAsync(order => order.UserId == userId && order.Id == orderId);
                if (order is null)
                {
                    return NotFound("Order not found with the given order id");
                }

                return Ok(order);
            }
        }

        [HttpPost("submitOrder")]
        public async Task<IActionResult> PostOrder()
        {
            var userId = Guid.Parse(HttpContext.User.Identity.Name);
            var cart = await _context.Carts.FindAsync(userId);
            if (cart is null)
            {
                return BadRequest("CartId not found");
            }

            // Create new order
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                QuantityByDrugs = cart.QuantityByDrugs,
                TotalPriceByDrugs = cart.TotalPriceByDrugs,
                TotalOrderPrice = cart.TotalCartAmount,
                OrderDate = DateTime.Now,
                OrderStatus = OrderStatus.ORDER_PLACED
            };

            // Validations
            foreach (var drug in cart.QuantityByDrugs)
            {
                var drugItem = await _context.Drugs.FindAsync(drug.Key);
                // Whether the drug is not present in the database
                if (drugItem is null)
                {
                    return BadRequest("Drug not found");
                }
                // Whether the quantity available is less than the quantity ordered
                if (drugItem.DrugQuantityAvailable < drug.Value)
                {
                    return BadRequest("Drug quantity not available");
                }
                drugItem.DrugQuantityAvailable -= drug.Value;
                _context.Drugs.Update(drugItem);
            }

            // Add order to the database
            _context.Orders.Add(order);
            // Empty the cart from the database
            _context.Carts.Remove(cart);

            await _context.SaveChangesAsync();

            return Created("Order created successfully", order.Id);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("updateOrderStatus/{orderId:Guid}")]
        public async Task<IActionResult> UpdateOrderStatus(Guid orderId, [FromQuery] OrderStatus orderStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order is null)
            {
                return NotFound("Order not found for the given id");
            }

            order.OrderStatus = orderStatus;
            await _context.SaveChangesAsync();

            return Ok("Order status updated successfully");
        }

        [HttpDelete("{orderId:Guid}")]
        public async Task<IActionResult> DeleteOrder(Guid orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order is null)
            {
                return NotFound("Order not found with the given id");
            }

            _context.Orders.Remove(order);

            foreach (var drugQuantityPair in order.QuantityByDrugs)
            {
                var drug = await _context.Drugs.FindAsync(drugQuantityPair.Key);
                drug.DrugQuantityAvailable += drugQuantityPair.Value;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
