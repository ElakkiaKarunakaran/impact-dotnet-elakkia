using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Reporting.Data;
using Xunit;

namespace Reporting.Tests;

// Task 10.4 — Reporting integration tests: real pipeline, EF InMemory swapped
// in for SQL Server. The `vw_EnrollmentSummary` view-backed endpoint is a real
// SQL Server view (EF DB First) and is validated live against the actual
// reporting DB during the Task 10.8 demo rather than here, since InMemory has
// no SQL view support; the Enrollments table endpoints cover the same auth path.
public class EnrollmentsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string JwtKey      = "Week10CapstoneSuperSecretKey_PortalMicroservices_2026!";
    private const string JwtIssuer   = "Week10.Identity";
    private const string JwtAudience = "Week10.Portal";

    private readonly HttpClient _client;

    public EnrollmentsControllerTests(WebApplicationFactory<Program> factory)
    {
        // See Academics.Tests/StudentsControllerTests.cs for why this is a real
        // process env var rather than a ConfigureAppConfiguration hook.
        Environment.SetEnvironmentVariable("UseInMemoryDb", "true");

        var customized = factory.WithWebHostBuilder(_ => { });
        _client = customized.CreateClient();

        using var scope = customized.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ReportingContext>();
        db.Enrollments.Add(new Enrollment { StudentName = "Elakkia", CourseCode = "CS101", EnrolledOn = DateTime.UtcNow });
        db.Enrollments.Add(new Enrollment { StudentName = "Ramya",   CourseCode = "CS101", EnrolledOn = DateTime.UtcNow });
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

    [Fact]
    public async Task GetAll_Unauthenticated_Returns401()
    {
        var resp = await _client.GetAsync("/api/enrollments");
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task GetAll_AuthenticatedTeacher_Returns200()
    {
        var req = new HttpRequestMessage(HttpMethod.Get, "/api/enrollments");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", MakeToken("Teacher"));
        var resp = await _client.SendAsync(req);
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task GetAll_AuthenticatedStudent_Returns200_ReadOnlyAllowed()
    {
        var req = new HttpRequestMessage(HttpMethod.Get, "/api/enrollments");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", MakeToken("Student"));
        var resp = await _client.SendAsync(req);
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task ByCourse_FiltersCorrectly()
    {
        var req = new HttpRequestMessage(HttpMethod.Get, "/api/enrollments/by-course/CS101");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", MakeToken("Teacher"));
        var resp = await _client.SendAsync(req);
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        var body = await resp.Content.ReadAsStringAsync();
        Assert.Contains("Elakkia", body);
        Assert.Contains("Ramya", body);
    }

    [Fact]
    public async Task Health_Returns200()
    {
        var resp = await _client.GetAsync("/api/reporting/health");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }
}
