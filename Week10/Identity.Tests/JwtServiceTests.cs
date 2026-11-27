using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Identity.Services;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Identity.Tests;

// Task 10.2 — JWT issuance carries the expected claims (sub, role).
public class JwtServiceTests
{
    private static JwtService MakeService()
    {
        var cfg = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"]      = "TestOnlySecretKeyForUnitTests_NotProd_2026!",
                ["Jwt:Issuer"]   = "Week10.Identity",
                ["Jwt:Audience"] = "Week10.Portal",
            })
            .Build();
        return new JwtService(cfg);
    }

    [Fact]
    public void Issue_Token_ContainsUsernameAndRoleClaims()
    {
        var jwt   = MakeService();
        var token = jwt.Issue("elakkia", "Teacher");

        var parsed = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal("elakkia", parsed.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal("Teacher", parsed.Claims.First(c => c.Type == ClaimTypes.Role).Value);
    }

    [Fact]
    public void Issue_Token_HasFutureExpiry()
    {
        var jwt   = MakeService();
        var token = jwt.Issue("elakkia", "Student");
        var parsed = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.True(parsed.ValidTo > DateTime.UtcNow);
    }
}
