using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Task 10.5 — YARP reads its routes/clusters straight from appsettings.json
// under the "ReverseProxy" section.
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Task 10.5 — the gateway validates the JWT ONCE, centrally, before any
// request reaches a downstream service. Role claims ride through unchanged
// inside the token, so each service can still re-check roles itself
// (defense in depth) without re-issuing or re-signing anything.
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"],
            ValidateAudience         = true,
            ValidAudience            = builder.Configuration["Jwt:Audience"],
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew                = TimeSpan.FromSeconds(30),
        };
    });

builder.Services.AddAuthorization(options =>
{
    // "Anonymous" is YARP's reserved policy name meaning "skip auth for this route" —
    // used for /identity/api/auth/register and /login, which must be reachable pre-token.
    // "Default" is ALSO reserved by YARP (its built-in authenticated-user policy), so the
    // custom policy below is named "RequireAuth" to avoid colliding with it.
    options.AddPolicy("RequireAuth", p => p.RequireAuthenticatedUser());
});

// Task 10.5 — React calls only the gateway; CORS is centralized here too.
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                     ?? new[] { "http://localhost:5173" };
builder.Services.AddCors(o => o.AddPolicy("ReactApp", p =>
    p.WithOrigins(allowedOrigins).AllowAnyHeader().WithMethods("GET", "POST", "PUT", "DELETE")));

var app = builder.Build();

app.UseCors("ReactApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { service = "gateway", status = "ok" }));

app.MapReverseProxy();

app.Run();

public partial class Program { }
