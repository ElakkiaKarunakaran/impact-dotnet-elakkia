using Microsoft.AspNetCore.Mvc;

namespace StudentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserStore _userStore;

    public AuthController(IUserStore userStore) => _userStore = userStore;

    // POST api/auth/login
    // Task 7.5 — login now hits whatever IUserStore is registered (ADO.NET / EF / InMemory)
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _userStore.FindByUsername(request.Username);
        if (user == null || user.PasswordHash != request.Password)
            return Unauthorized("Invalid username or password");

        return Ok(new { user.Id, user.Username, user.Role, Message = "Login successful" });
    }
}

public record LoginRequest(string Username, string Password);
