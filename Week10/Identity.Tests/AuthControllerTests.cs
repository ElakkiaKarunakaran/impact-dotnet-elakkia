using System.Net;
using System.Net.Http.Json;
using Identity.Data;
using Identity.Models;
using Identity.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Identity.Tests;

// Task 10.2 — integration tests over the real pipeline, IUserRepository mocked
// so no live SQL Server is needed to run this suite.
public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly Mock<IUserRepository> _repoMock = new();
    private readonly HttpClient _client;

    public AuthControllerTests(WebApplicationFactory<Program> factory)
    {
        var customized = factory.WithWebHostBuilder(host =>
            host.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IUserRepository));
                if (descriptor is not null) services.Remove(descriptor);
                services.AddSingleton(_repoMock.Object);
            }));
        _client = customized.CreateClient();
    }

    [Fact]
    public async Task Register_NewUsername_Returns201()
    {
        _repoMock.Setup(r => r.FindByUsername("newuser")).Returns((User?)null);

        var resp = await _client.PostAsJsonAsync("/api/auth/register",
            new RegisterDto("newuser", "password123", "Student"));

        Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
        _repoMock.Verify(r => r.Insert(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task Register_DuplicateUsername_Returns409()
    {
        _repoMock.Setup(r => r.FindByUsername("existing"))
                 .Returns(new User { Username = "existing", Role = "Student" });

        var resp = await _client.PostAsJsonAsync("/api/auth/register",
            new RegisterDto("existing", "password123", "Student"));

        Assert.Equal(HttpStatusCode.Conflict, resp.StatusCode);
    }

    [Fact]
    public async Task Register_ShortPassword_Returns400()
    {
        var resp = await _client.PostAsJsonAsync("/api/auth/register",
            new RegisterDto("shortpw", "123", "Student"));

        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_Returns200WithToken()
    {
        var (hash, salt) = PasswordHasher.Hash("correctpassword");
        _repoMock.Setup(r => r.FindByUsername("teacher1"))
                 .Returns(new User { Username = "teacher1", PasswordHash = hash, PasswordSalt = salt, Role = "Teacher" });

        var resp = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginDto("teacher1", "correctpassword"));

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var body = await resp.Content.ReadFromJsonAsync<TokenDto>();
        Assert.False(string.IsNullOrWhiteSpace(body!.Token));
        Assert.Equal("Teacher", body.Role);
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        var (hash, salt) = PasswordHasher.Hash("correctpassword");
        _repoMock.Setup(r => r.FindByUsername("teacher1"))
                 .Returns(new User { Username = "teacher1", PasswordHash = hash, PasswordSalt = salt, Role = "Teacher" });

        var resp = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginDto("teacher1", "wrongpassword"));

        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task Login_UnknownUsername_Returns401()
    {
        _repoMock.Setup(r => r.FindByUsername("ghost")).Returns((User?)null);

        var resp = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginDto("ghost", "whatever"));

        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task Health_Returns200()
    {
        var resp = await _client.GetAsync("/api/auth/health");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }
}
