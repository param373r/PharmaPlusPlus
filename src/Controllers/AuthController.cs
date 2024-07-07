using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaPlusPlus.Data;
using PharmaPlusPlus.Models;
using PharmaPlusPlus.Models.Contracts;
using PharmaPlusPlus.Services;

namespace PharmaPlusPlus.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private readonly PasswordHasher<object> _passwordHasher = new();
    private readonly PharmaPlusPlusContext _context;
    private readonly IJwtService _jwtService;

    public AuthController(PharmaPlusPlusContext dbContext, IJwtService jwtService)
    {
        _context = dbContext;
        _jwtService = jwtService;
    }

    [HttpPost("register/self")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        string pass = _passwordHasher.HashPassword(null, request.Password);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserEmail = request.UserEmail,
            Password = pass
        };

        User users = _context.Users.Where(u => u.UserEmail == user.UserEmail).FirstOrDefault();
        if (users is not null)
        {
            return BadRequest(new Response { IsSuccess = false, Message = "Email Already Exists Please Login" });
        }

        var result = await _context.Users.AddAsync(user);

        await _context.SaveChangesAsync();

        return Ok(new RegisterResponse { UserId = user.Id });
    }

    [HttpPost("register/admin")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        string pass = _passwordHasher.HashPassword(null, request.Password);
        var user = new User
        {
            Id = request.Id ?? Guid.NewGuid(),
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserEmail = request.UserEmail,
            Password = pass,
            Role = request.Role ?? Role.User
        };

        User existingUser = _context.Users.Where(u => u.UserEmail == user.UserEmail).FirstOrDefault();
        if (existingUser is not null)
        {
            return BadRequest(new Response { IsSuccess = false, Message = "Email Already Exists Please Login" });
        }

        var result = await _context.Users.AddAsync(user);

        await _context.SaveChangesAsync();

        return Ok(new RegisterResponse { UserId = user.Id });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserEmail == request.UserEmail);

        if (user == null)
        {
            return BadRequest(new Response { IsSuccess = false, Message = "User couldn't be found in database." });
        }

        var result = _passwordHasher.VerifyHashedPassword(null, user.Password, request.Password);
        if (result == PasswordVerificationResult.Success)
        {
            var token = _jwtService.GenerateJwtToken(user);
            return Ok(new LoginResponse { IsSuccess = true, Token = token, Message = "Authentication successful." });
        }
        return Unauthorized(new Response { IsSuccess = false, Message = "Credentials do not match." });
    }

    [HttpPost("changePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserEmail == request.UserEmail);

        if (user == null)
        {
            return BadRequest(new Response { IsSuccess = false, Message = "User couldn't be found in database." });
        }
        if (user.Id != Guid.Parse(HttpContext.User.Identity.Name) && !HttpContext.User.IsInRole("Admin"))
        {
            return Unauthorized(new Response { IsSuccess = false, Message = "You are not authorized to change this user's password." });
        }

        var result = _passwordHasher.VerifyHashedPassword(null, user.Password, request.OldPassword);
        if (result == PasswordVerificationResult.Success)
        {
            user.Password = _passwordHasher.HashPassword(null, request.NewPassword);
            await _context.SaveChangesAsync();
            return Ok(new Response { IsSuccess = true, Message = "Password changed successfully." });
        }
        return Unauthorized(new Response { IsSuccess = false, Message = "Old password is incorrect." });
    }

    [HttpPost("changeEmail/{userId:Guid}")]
    public async Task<IActionResult> ChangeEmail(Guid userId, [FromBody] ChangeEmailRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (userId != Guid.Parse(HttpContext.User.Identity.Name) && !HttpContext.User.IsInRole("Admin"))
        {
            return Unauthorized(new Response { IsSuccess = false, Message = "You are not authorized to change this user's email." });
        }

        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return BadRequest(new Response { IsSuccess = false, Message = "User couldn't be found in database." });
        }

        user.UserEmail = request.NewEmail;
        await _context.SaveChangesAsync();
        return Ok(new Response { IsSuccess = true, Message = "Email changed successfully." });
    }
}