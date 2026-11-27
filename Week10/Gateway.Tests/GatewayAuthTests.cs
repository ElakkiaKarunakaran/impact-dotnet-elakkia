using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Gateway.Tests;

// Task 10.5 / Testing Focus — proves the gateway itself rejects a tampered
// JWT BEFORE it ever reaches a downstream service. The academics/reporting
// clusters are unreachable in this test host (no containers running), so a
// request that gets past authentication would surface as a proxy/connection
// error, not a 401 — the assertions below pin the auth check specifically.
public class GatewayAuthTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string RealKey   = "Week10CapstoneSuperSecretKey_PortalMicroservices_2026!";
    private const string WrongKey  = "ThisIsADifferentKeyAnAttackerMightTry_1234567890!";
    private const string Issuer    = "Week10.Identity";
    private const string Audience  = "Week10.Portal";

    private readonly HttpClient _client;

    public GatewayAuthTests(WebApplicationFactory<Program> factory)
        => _client = factory.CreateClient();

    private static string MakeToken(string signingKey, string role = "Teacher")
    {
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "attacker"),
            new Claim(ClaimTypes.Role, role),
        };
        var token = new JwtSecurityToken(Issuer, Audience, claims,
            expires: DateTime.UtcNow.AddHours(1), signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Fact]
    public async Task TamperedSignature_SignedWithWrongKey_Returns401AtGateway()
    {
        var tampered = MakeToken(WrongKey);
        var req = new HttpRequestMessage(HttpMethod.Get, "/academics/api/students");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tampered);

        var resp = await _client.SendAsync(req);

        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task TruncatedSignature_BitFlippedToken_Returns401AtGateway()
    {
        var valid = MakeToken(RealKey);
        // Flip the last character of the signature segment — still well-formed, still invalid.
        var tampered = valid[..^1] + (valid[^1] == 'A' ? 'B' : 'A');

        var req = new HttpRequestMessage(HttpMethod.Get, "/academics/api/students");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tampered);

        var resp = await _client.SendAsync(req);

        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task MissingToken_ProtectedRoute_Returns401AtGateway()
    {
        var resp = await _client.GetAsync("/academics/api/students");
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task ValidSignature_PassesGatewayAuth_DoesNotReturn401()
    {
        var valid = MakeToken(RealKey);
        var req = new HttpRequestMessage(HttpMethod.Get, "/academics/api/students");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", valid);

        var resp = await _client.SendAsync(req);

        // Academics isn't running in this test host, so the proxy hop itself
        // fails (502/503) — but auth must NOT be what rejected it.
        Assert.NotEqual(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task Health_IsPublic_Returns200()
    {
        var resp = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }
}
