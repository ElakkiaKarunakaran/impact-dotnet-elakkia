using Identity.Data;
using Identity.Models;
using Identity.Services;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers;

// Task 10.2 — register / login endpoints the gateway exposes at /identity/*.
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _users;
    private readonly JwtService      _jwt;

    public AuthController(IUserRepository users, JwtService jwt)
    {
        _users = users;
        _jwt   = jwt;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || dto.Password.Length < 6)
            return BadRequest("Username required; password at least 6 chars.");

        if (_users.FindByUsername(dto.Username) is not null)
            return Conflict($"Username '{dto.Username}' already exists.");

        var (hash, salt) = PasswordHasher.Hash(dto.Password);
        var user = new User
        {
            Username     = dto.Username,
            PasswordHash = hash,
            PasswordSalt = salt,
            Role         = dto.Role ?? "Student",
        };
        _users.Insert(user);

        return CreatedAtAction(nameof(Register),
            new { id = user.Id },
            new { user.Id, user.Username, user.Role });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        var user = _users.FindByUsername(dto.Username);
        if (user is null)
            return Unauthorized("Invalid credentials.");

        if (!PasswordHasher.Verify(dto.Password, user.PasswordHash, user.PasswordSalt))
            return Unauthorized("Invalid credentials.");

        var token = _jwt.Issue(user.Username, user.Role);
        return Ok(new TokenDto(token, user.Username, user.Role));
    }

    [HttpGet("health")]
    public IActionResult Health() => Ok(new { service = "identity", status = "ok" });
}
