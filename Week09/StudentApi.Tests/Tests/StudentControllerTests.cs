using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Xunit;

// Task 9.3 — consolidated backend integration tests
// Covers: auth, role-based access, CRUD, error paths.
// Runs against the real .NET pipeline via WebApplicationFactory<Program>.

public class StudentControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    // JWT config must match Week07/StudentApi/appsettings.json
    private const string JwtKey      = "WeekEightSuperSecretKey2026ForJwtBearerAuth!";
    private const string JwtIssuer   = "StudentApi";
    private const string JwtAudience = "StudentPortal";

    public StudentControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private static string MakeToken(string role, string username = "testuser")
    {
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(ClaimTypes.Role, role),
            new Claim("userId", "1"),
        };
        var token = new JwtSecurityToken(
            issuer:            JwtIssuer,
            audience:          JwtAudience,
            claims:            claims,
            expires:           DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private HttpRequestMessage Req(HttpMethod method, string path, string role,
                                   string? body = null, string contentType = "application/json")
    {
        var req = new HttpRequestMessage(method, path);
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", MakeToken(role));
        if (body is not null)
            req.Content = new StringContent(body, Encoding.UTF8, contentType);
        return req;
    }

    // ── Authentication guard ────────────────────────────────────────────────

    // Task 9.2 — unauthenticated GET → 401
    [Fact]
    public async Task GetAll_Unauthenticated_Returns401()
    {
        var resp = await _client.GetAsync("/api/students");
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    // Task 9.1 — authenticated Teacher GET → 200
    [Fact]
    public async Task GetAll_AuthenticatedTeacher_Returns200()
    {
        var resp = await _client.SendAsync(Req(HttpMethod.Get, "/api/students", "Teacher"));
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    // Task 9.1 — authenticated Student GET → 200 (read-only allowed)
    [Fact]
    public async Task GetAll_AuthenticatedStudent_Returns200()
    {
        var resp = await _client.SendAsync(Req(HttpMethod.Get, "/api/students", "Student"));
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    // ── Canonical test 1 — valid create → 201 ──────────────────────────────

    [Fact]
    public async Task AddStudent_Teacher_ValidInput_Returns201()
    {
        var body = """{"name":"CanonicalUser","age":21,"rollNumber":"CN001","email":"cn@test.com"}""";
        var resp = await _client.SendAsync(Req(HttpMethod.Post, "/api/students", "Teacher", body));
        Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
    }

    // ── Canonical test 2 — missing / nonexistent id → 404 ──────────────────

    [Fact]
    public async Task GetById_NonExistentId_Returns404()
    {
        var resp = await _client.SendAsync(Req(HttpMethod.Get, "/api/students/99999", "Teacher"));
        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }

    // ── Canonical test 3 — invalid input → 400 ─────────────────────────────

    [Fact]
    public async Task AddStudent_Teacher_EmptyName_Returns400()
    {
        var body = """{"name":"","age":21,"rollNumber":"CN002","email":"cn2@test.com"}""";
        var resp = await _client.SendAsync(Req(HttpMethod.Post, "/api/students", "Teacher", body));
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task AddStudent_Teacher_InvalidAge_Returns400()
    {
        var body = """{"name":"AgeTest","age":200,"rollNumber":"CN003","email":"cn3@test.com"}""";
        var resp = await _client.SendAsync(Req(HttpMethod.Post, "/api/students", "Teacher", body));
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    // ── Canonical test 4 — simulated repo error → 500 ──────────────────────

    [Fact]
    public async Task GetAll_RepoThrows_Returns500()
    {
        // Build a factory that replaces IRepository<Student> with a throwing mock
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(host =>
                host.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IRepository<Student>));
                    if (descriptor is not null)
                        services.Remove(descriptor);

                    var mock = new Mock<IRepository<Student>>();
                    mock.Setup(r => r.GetAll())
                        .Throws(new InvalidOperationException("Simulated DB failure"));
                    services.AddSingleton(mock.Object);
                }));

        var client = factory.CreateClient();
        var req = new HttpRequestMessage(HttpMethod.Get, "/api/students");
        req.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", MakeToken("Teacher"));
        var resp = await client.SendAsync(req);

        Assert.Equal(HttpStatusCode.InternalServerError, resp.StatusCode);
    }

    // ── Canonical test 5 — Student blocked on write, Teacher allowed ────────

    // Task 9.2 — Student POST → 403
    [Fact]
    public async Task AddStudent_Student_Returns403()
    {
        var body = """{"name":"Blocked","age":20,"rollNumber":"BK001","email":"bk@test.com"}""";
        var resp = await _client.SendAsync(Req(HttpMethod.Post, "/api/students", "Student", body));
        Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
    }

    // Task 9.2 — Student DELETE → 403
    [Fact]
    public async Task DeleteStudent_Student_Returns403()
    {
        var resp = await _client.SendAsync(Req(HttpMethod.Delete, "/api/students/1", "Student"));
        Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
    }

    // Task 9.2 — Student PUT → 403
    [Fact]
    public async Task UpdateStudent_Student_Returns403()
    {
        var body = """{"name":"Hacker","age":20,"rollNumber":"HK001","email":"hk@test.com"}""";
        var resp = await _client.SendAsync(Req(HttpMethod.Put, "/api/students/1", "Student", body));
        Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
    }

    // ── Additional service-layer tests (mocked repo, no HTTP pipeline) ──────

    [Fact]
    public void Service_AddStudent_ValidStudent_ReturnsSuccess()
    {
        var mock = new Mock<IRepository<Student>>();
        mock.Setup(r => r.GetAll()).Returns(new List<Student>());
        var sut = new StudentService(mock.Object);

        var result = sut.AddStudent(new Student
            { Name = "Portal", Age = 20, RollNumber = "P001", Email = "p@test.com" });

        Assert.True(result.Success);
        mock.Verify(r => r.Add(It.IsAny<Student>()), Times.Once);
    }

    [Fact]
    public void Service_GetById_Missing_ReturnsNull()
    {
        var mock = new Mock<IRepository<Student>>();
        mock.Setup(r => r.GetById(99)).Returns((Student?)null);
        var sut = new StudentService(mock.Object);

        Assert.Null(sut.GetById(99));
    }

    [Fact]
    public void Service_Delete_Missing_ReturnsFail()
    {
        var mock = new Mock<IRepository<Student>>();
        mock.Setup(r => r.GetById(It.IsAny<int>())).Returns((Student?)null);
        var sut = new StudentService(mock.Object);

        var result = sut.DeleteStudent(42);

        Assert.False(result.Success);
        Assert.Contains("42", result.Message);
    }
}
