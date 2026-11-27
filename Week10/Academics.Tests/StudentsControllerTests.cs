using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Academics.Data;
using Academics.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Academics.Tests;

// Task 10.3 — Academics integration tests: real pipeline, EF InMemory swapped
// in for SQL Server so this suite needs no live database.
public class StudentsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string JwtKey      = "Week10CapstoneSuperSecretKey_PortalMicroservices_2026!";
    private const string JwtIssuer   = "Week10.Identity";
    private const string JwtAudience = "Week10.Portal";

    private readonly HttpClient _client;

    public StudentsControllerTests(WebApplicationFactory<Program> factory)
    {
        // Task 10.3 — swap in EF InMemory via the app's own "UseInMemoryDb"
        // config switch (Program.cs), so only ONE EF provider is ever
        // registered — no provider-conflict, no descriptor surgery needed.
        // Set as a real process environment variable (not ConfigureAppConfiguration)
        // because Program.cs reads this BEFORE builder.Build() runs — at which
        // point WebApplicationFactory's own config-injection hooks haven't fired
        // yet — while builder.Configuration's AddEnvironmentVariables() provider
        // reads the live process environment at that same early point.
        Environment.SetEnvironmentVariable("UseInMemoryDb", "true");

        var customized = factory.WithWebHostBuilder(_ => { });
        _client = customized.CreateClient();

        using var scope = customized.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AcademicsContext>();
        db.Students.Add(new Student { Name = "Seed", Age = 20, RollNumber = "SEED1", Email = "seed@test.com" });
        db.SaveChanges();
    }

    private static string MakeToken(string role, string username = "testuser")
    {
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(ClaimTypes.Role, role),
        };
        var token = new JwtSecurityToken(JwtIssuer, JwtAudience, claims,
            expires: DateTime.UtcNow.AddHours(1), signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private HttpRequestMessage Req(HttpMethod method, string path, string? role, object? body = null)
    {
        var req = new HttpRequestMessage(method, path);
        if (role is not null)
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", MakeToken(role));
        if (body is not null)
            req.Content = JsonContent.Create(body);
        return req;
    }

    [Fact]
    public async Task GetAll_Unauthenticated_Returns401()
    {
        var resp = await _client.SendAsync(Req(HttpMethod.Get, "/api/students", null));
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task GetAll_Teacher_Returns200()
    {
        var resp = await _client.SendAsync(Req(HttpMethod.Get, "/api/students", "Teacher"));
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task GetAll_Student_Returns200_ReadOnlyAllowed()
    {
        var resp = await _client.SendAsync(Req(HttpMethod.Get, "/api/students", "Student"));
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task Add_Teacher_ValidStudent_Returns201()
    {
        var body = new { Name = "Canonical", Age = 21, RollNumber = "CN900", Email = "cn900@test.com" };
        var resp = await _client.SendAsync(Req(HttpMethod.Post, "/api/students", "Teacher", body));
        Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
    }

    [Fact]
    public async Task Add_Student_Returns403()
    {
        var body = new { Name = "Blocked", Age = 20, RollNumber = "BK900", Email = "bk900@test.com" };
        var resp = await _client.SendAsync(Req(HttpMethod.Post, "/api/students", "Student", body));
        Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
    }

    [Fact]
    public async Task Add_DuplicateRollNumber_Returns400()
    {
        var body = new { Name = "Dup", Age = 20, RollNumber = "SEED1", Email = "dup@test.com" };
        var resp = await _client.SendAsync(Req(HttpMethod.Post, "/api/students", "Teacher", body));
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task GetById_NonExistentId_Returns404()
    {
        var resp = await _client.SendAsync(Req(HttpMethod.Get, "/api/students/999999", "Teacher"));
        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }

    [Fact]
    public async Task Delete_Student_Returns403()
    {
        var resp = await _client.SendAsync(Req(HttpMethod.Delete, "/api/students/1", "Student"));
        Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
    }

    [Fact]
    public async Task Health_Returns200()
    {
        var resp = await _client.GetAsync("/api/academics/health");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }
}
