using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Academics.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Academics.Tests;

// Task 10.3 — Courses and Teachers endpoints, same auth rules as Students.
public class CoursesAndTeachersControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string JwtKey      = "Week10CapstoneSuperSecretKey_PortalMicroservices_2026!";
    private const string JwtIssuer   = "Week10.Identity";
    private const string JwtAudience = "Week10.Portal";

    private readonly HttpClient _client;

    public CoursesAndTeachersControllerTests(WebApplicationFactory<Program> factory)
    {
        Environment.SetEnvironmentVariable("UseInMemoryDb", "true");
        var customized = factory.WithWebHostBuilder(_ => { });
        _client = customized.CreateClient();

        using var scope = customized.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AcademicsContext>();
        if (!db.Teachers.Any())
            db.Teachers.Add(new Academics.Models.Teacher { Name = "Dr. Seed", Department = "CS", Email = "seed-teacher@test.com" });
        db.SaveChanges();
    }

    private static string MakeToken(string role)
    {
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "testuser"),
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
    public async Task Courses_GetAll_Unauthenticated_Returns401()
    {
        var resp = await _client.SendAsync(Req(HttpMethod.Get, "/api/courses", null));
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task Courses_GetAll_Authenticated_Returns200()
    {
        var resp = await _client.SendAsync(Req(HttpMethod.Get, "/api/courses", "Student"));
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task Courses_Add_Teacher_Returns201()
    {
        var body = new { Code = "CS999", Title = "Capstone Seminar", Credits = 3 };
        var resp = await _client.SendAsync(Req(HttpMethod.Post, "/api/courses", "Teacher", body));
        Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
    }

    [Fact]
    public async Task Courses_Add_Student_Returns403()
    {
        var body = new { Code = "CS998", Title = "Blocked Course", Credits = 3 };
        var resp = await _client.SendAsync(Req(HttpMethod.Post, "/api/courses", "Student", body));
        Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
    }

    [Fact]
    public async Task Teachers_GetAll_Unauthenticated_Returns401()
    {
        var resp = await _client.SendAsync(Req(HttpMethod.Get, "/api/teachers", null));
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task Teachers_GetAll_Authenticated_Returns200()
    {
        var resp = await _client.SendAsync(Req(HttpMethod.Get, "/api/teachers", "Teacher"));
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }
}
