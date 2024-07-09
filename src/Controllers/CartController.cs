using Microsoft.AspNetCore.Mvc;
using PharmaPlusPlus.Data;
using PharmaPlusPlus.Models;
using Microsoft.EntityFrameworkCore;
using PharmaPlusPlus.Models.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace PharmaPlusPlus.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    public class CartController : ControllerBase
    {
        private readonly PharmaPlusPlusContext _context;
        public CartController(PharmaPlusPlusContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(AddCartRequest addCartModel)
        {
            var userId = Guid.Parse(HttpContext.User.Identity.Name);
            var user = await _context.Users.FindAsync(userId);
            var drugId = addCartModel.DrugId;
            var quantity = addCartModel.RequiredQuantity;

            var drug = await _context.Drugs.FindAsync(drugId);

            // Check whether drug exists in the database
            if (drug is null)
            {
                return BadRequest("Drug not found");
            }

            // Check whether the quantity requested is available
            else if (drug is not null && drug.DrugQuantityAvailable < quantity)
            {
                return BadRequest("Not enough stock");
            }

            // Check if cart already exists
            var cart = await _context.Carts.FirstOrDefaultAsync(cart => cart.UserCartId == user.Id);
            // Create a cart with the userId and add the drug to the cart
            if (cart is null)
            {
                cart = new Cart
                {
                    UserCartId = user.Id,
                    QuantityByDrugs = new Dictionary<Guid, int> { { drugId, quantity } },
                    TotalPriceByDrugs = new Dictionary<Guid, double> { { drugId, drug.DrugPrice * quantity } },
                    TotalCartAmount = drug.DrugPrice * quantity,
                    DateAdded = DateTime.Now,
                    DateModified = DateTime.Now
                };
                _context.Carts.Add(cart);
            }
            else
            {
                // If cart already exists, add the drug to the cart
                // Update the quantity and total price of the drug in the cart
                cart.QuantityByDrugs[drugId] = cart.QuantityByDrugs.GetValueOrDefault(drugId, 0) + quantity;
                if (cart.QuantityByDrugs[drugId] > drug.DrugQuantityAvailable)
                {
                    return BadRequest("Not enough stock");
                }
                cart.TotalPriceByDrugs[drugId] = cart.TotalPriceByDrugs.GetValueOrDefault(drugId, 0) + drug.DrugPrice * quantity;
                cart.TotalCartAmount += drug.DrugPrice * quantity;
                cart.DateModified = DateTime.Now;
                _context.Carts.Update(cart);
            }

            await _context.SaveChangesAsync();

            return Ok("Product added to cart");
        }

        [HttpGet()]
        public async Task<IActionResult> GetCartItems()
        {
            var userId = Guid.Parse(HttpContext.User.Identity.Name);
            var cart = await _context.Carts.FirstOrDefaultAsync(cart => cart.UserCartId == userId);

            if (cart is null)
            {
                return BadRequest("No items in the cart");
            }

            return Ok(cart);
        }

        [HttpDelete("deleteItem/{drugId:Guid}")]
        public async Task<IActionResult> RemoveFromCart(Guid drugId,[FromQuery] int quantity)
        {
            var userId = Guid.Parse(HttpContext.User.Identity.Name);
            var cart = await _context.Carts.FindAsync(userId);

            if (cart is null)
            {
                return BadRequest("Cart is already empty.");
            }

            // If contains the drug, remove the drug and update other properties.
            if (cart.QuantityByDrugs.ContainsKey(drugId))
            {
                // If the quantity of the drug in cart is greater
                // remove the drugs by quantity and update other props
                if (cart.QuantityByDrugs[drugId] > quantity)
                {
                    Drug drug = await _context.Drugs.FindAsync(drugId);
                    cart.QuantityByDrugs[drugId] -= quantity;
                    var totalAmountRemoved = quantity * drug.DrugPrice;
                    cart.TotalPriceByDrugs[drugId] -= totalAmountRemoved;
                    cart.TotalCartAmount -= totalAmountRemoved;
                }
                // If the quantity of the drug in cart is equal or lesser
                // remove the drug completely and update the props.
                else
                {
                    cart.QuantityByDrugs.Remove(drugId);
                    if (cart.QuantityByDrugs.Count == 0)
                    {
                        _context.Carts.Remove(cart);
                        await _context.SaveChangesAsync();
                        return Ok("Cart cleared successfully");
                    }
                    cart.TotalCartAmount -= cart.TotalPriceByDrugs[drugId];
                    cart.TotalPriceByDrugs.Remove(drugId);
                }

                cart.DateModified = DateTime.Now;
                _context.Carts.Update(cart);
                await _context.SaveChangesAsync();

                return Ok("Product removed from cart");
            }
            else
            {
                return BadRequest("Product not found in cart");
            }
        }
        
        [HttpDelete("deleteCart")]
        public async Task<IActionResult> EmptyCart(Guid cartId)
        {
            var cart = await _context.Carts.FindAsync(cartId);

            if (cart is null)
            {
                return BadRequest("Cart is already empty.");
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return Ok("Cart cleared successfully");
        }
    }
}