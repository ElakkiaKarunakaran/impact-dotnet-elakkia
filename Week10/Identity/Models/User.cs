namespace Identity.Models;

// Task 10.2 — ADO.NET user record, security-critical service.
public class User
{
    public int    Id           { get; set; }
    public string Username     { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;  // PBKDF2 output
    public string PasswordSalt { get; set; } = string.Empty;  // base64 random salt
    public string Role         { get; set; } = "Student";
}

public record RegisterDto(string Username, string Password, string? Role);
public record LoginDto(string Username, string Password);
public record TokenDto(string Token, string Username, string Role);
