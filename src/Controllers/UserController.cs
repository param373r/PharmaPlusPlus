using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaPlusPlus.Data;
using PharmaPlusPlus.Models;
using PharmaPlusPlus.Models.Contracts;

namespace PharmaPlusPlus.Controllers
{

    [ApiController]
    [Authorize(Roles = "Admin,User")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly PharmaPlusPlusContext _context;

        public UserController(PharmaPlusPlusContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("{userId:Guid}")]
        public async Task<ActionResult<User>> GetUser(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPut("{userId:Guid}")]
        public async Task<IActionResult> UpdateUser(Guid userId, UpdateUserRequest request)
        {
            var jwtUserId = Guid.Parse(HttpContext.User.Identity.Name);
            if (userId == jwtUserId || HttpContext.User.IsInRole(Role.Admin.ToString()))
            {
                var user = await _context.Users.FindAsync(userId);
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Username = request.Username;

                await _context.SaveChangesAsync();

                return NoContent();
            }

            return Unauthorized(new Response { Message = "Unauthorized" });
        }

        [HttpDelete("{userId:Guid}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound(new Response { IsSuccess = false, Message = "User not found" });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

