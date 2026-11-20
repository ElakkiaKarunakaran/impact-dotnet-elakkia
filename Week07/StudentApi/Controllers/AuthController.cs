using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace StudentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserStore _userStore;
    private readonly IConfiguration _config;

    public AuthController(IUserStore userStore, IConfiguration config)
    {
        _userStore = userStore;
        _config    = config;
    }

    // Task 8.6 + 8.7 — login returns a JWT
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _userStore.FindByUsername(request.Username);
        if (user == null || user.PasswordHash != HashPassword(request.Password))
            return Unauthorized("Invalid username or password");

        var token = GenerateJwt(user);
        return Ok(new { token, role = user.Role, username = user.Username });
    }

    // Task 8.5 — register new user
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Username and password are required");

        if (_userStore.FindByUsername(request.Username) != null)
            return Conflict("Username already taken");

        var user = new User
        {
            Username     = request.Username,
            PasswordHash = HashPassword(request.Password),
            Role         = request.Role ?? "Student"
        };
        _userStore.Add(user);
        return Ok(new { user.Username, user.Role, Message = "Registration successful" });
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private static string HashPassword(string password)
        => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));

    private string GenerateJwt(User user)
    {
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role,              user.Role),
            new Claim("userId",                     user.Id.ToString())
        };

        var expiry = int.TryParse(_config["Jwt:ExpiryMinutes"], out var m) ? m : 60;

        var token = new JwtSecurityToken(
            issuer:            _config["Jwt:Issuer"],
            audience:          _config["Jwt:Audience"],
            claims:            claims,
            expires:           DateTime.UtcNow.AddMinutes(expiry),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record LoginRequest(string Username, string Password);
public record RegisterRequest(string Username, string Password, string? Role);
