using OpenQA.Selenium;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace StudentApi.E2E.Tests;

/// <summary>
/// End-to-end browser tests covering the full portal journey.
///
/// Tasks covered:
///   9.5  — Smoke test: app loads and shows login page
///   9.6  — Register + login (JWT session validation)
///   9.7  — Teacher CRUD, Student read-only, Logout
///   9.8  — Negative: Student write attempt rejected by the server (UI + API)
/// </summary>
public class PortalE2ETests : SeleniumBase
{
    // ── Task 9.5 — Smoke test ───────────────────────────────────────────────

    [Fact]
    public void SmokeTest_AppLoads_ShowsLoginPage()
    {
        Driver.Navigate().GoToUrl($"{AppUrl}/login");
        Wait.Until(d => d.FindElement(By.TagName("h2")));

        var heading = Driver.FindElement(By.TagName("h2")).Text;
        Assert.Contains("Login", heading, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SmokeTest_UnauthenticatedVisit_RedirectsToLogin()
    {
        Driver.Navigate().GoToUrl($"{AppUrl}/students");
        Wait.Until(d => d.Url.Contains("/login"));
        Assert.Contains("/login", Driver.Url);
    }

    // ── Task 9.6 — Register + Login ─────────────────────────────────────────

    [Fact]
    public void RegisterAndLogin_ValidCredentials_ShowsStudentsPage()
    {
        // Register a fresh user (unique name avoids conflicts)
        var username = $"e2e{DateTime.Now.Ticks % 100000}";

        Driver.Navigate().GoToUrl($"{AppUrl}/register");
        Wait.Until(d => d.FindElement(By.Id("name")));

        Driver.FindElement(By.Id("name")).SendKeys("E2E Tester");
        SetDateField("dob", "2000-06-15");
        Driver.FindElement(By.Id("designation")).SendKeys("Student");
        Driver.FindElement(By.Id("email")).SendKeys($"{username}@test.com");
        Driver.FindElement(By.Id("password")).SendKeys("pass123!");

        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        // After registration the page shows a success message, then redirects to login
        Wait.Until(d =>
            d.FindElements(By.CssSelector("[role='status']")).Any() ||
            d.Url.Contains("/login"));

        // Login with the newly registered account
        Driver.Navigate().GoToUrl($"{AppUrl}/login");
        Wait.Until(d => d.FindElement(By.Id("username")));

        Driver.FindElement(By.Id("username")).SendKeys(username);
        Driver.FindElement(By.Id("password")).SendKeys("pass123!");
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        Wait.Until(d => d.Url.Contains("/students"));
        Assert.Contains("/students", Driver.Url);
    }

    // ── Task 9.7 — Teacher CRUD ─────────────────────────────────────────────

    [Fact]
    public void Teacher_AddStudent_StudentAppearsInList()
    {
        Login("admin", "admin123!");

        // Add Student button should be visible for Teacher
        var createBtn = WaitFor("[data-testid='create-btn']");
        Assert.True(createBtn.Displayed);
        createBtn.Click();

        // Fill the add-student form
        Wait.Until(d => d.FindElement(By.CssSelector("input[placeholder='Name']")));
        var suffix = DateTime.Now.Ticks % 10000;
        Driver.FindElement(By.CssSelector("input[placeholder='Name']")).SendKeys($"E2EStudent{suffix}");
        Driver.FindElement(By.CssSelector("input[placeholder='Age']")).SendKeys("22");
        Driver.FindElement(By.CssSelector("input[placeholder='Roll No.']")).SendKeys($"E{suffix}");
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        // The new student should appear in the table
        Wait.Until(d => d.FindElements(By.XPath($"//td[contains(text(),'E2EStudent{suffix}')]")).Any());
        Assert.NotEmpty(Driver.FindElements(By.XPath($"//td[contains(text(),'E2EStudent{suffix}')]")));
    }

    // ── Task 9.7 — Student read-only ────────────────────────────────────────

    [Fact]
    public void Student_CannotSeeCreateButton_AndNoEditDeleteButtons()
    {
        Login("elakkia", "elakkia123!");

        // Wait for the students table to load
        Wait.Until(d => d.FindElements(By.CssSelector("table")).Any() ||
                        d.FindElements(By.CssSelector("[aria-label='loading']")).Any());

        // Wait until loading is gone
        Wait.Until(d => !d.FindElements(By.CssSelector("[aria-label='loading']")).Any());

        Assert.Empty(Driver.FindElements(By.CssSelector("[data-testid='create-btn']")));
        Assert.Empty(Driver.FindElements(By.XPath("//button[contains(text(),'Edit')]")));
        Assert.Empty(Driver.FindElements(By.XPath("//button[contains(text(),'Delete')]")));
    }

    // ── Task 9.7 — Logout ───────────────────────────────────────────────────

    [Fact]
    public void Logout_ClearsSessionAndRedirectsToLogin()
    {
        Login("admin", "admin123!");

        // Click Logout button (btn-lime class in the nav)
        var logoutBtn = WaitFor("button.btn-lime");
        logoutBtn.Click();

        Wait.Until(d => d.Url.Contains("/login"));
        Assert.Contains("/login", Driver.Url);
    }

    // ── Task 9.8 — Negative: Student write rejected ──────────────────────────

    /// <summary>
    /// Proves the SERVER returns 403 when a Student calls a Teacher endpoint
    /// directly — bypassing the hidden button (Task 9.2 / 9.8).
    ///
    /// Browser half: Student UI shows no create button (enforced by React).
    /// API half:     Student credentials → POST /api/students → 403 Forbidden.
    /// </summary>
    [Fact]
    public async Task Student_DirectApiWriteAttempt_Returns403()
    {
        // ── Browser: verify no create button for Student ──────────────────
        Login("elakkia", "elakkia123!");
        Wait.Until(d => !d.FindElements(By.CssSelector("[aria-label='loading']")).Any());
        Assert.Empty(Driver.FindElements(By.CssSelector("[data-testid='create-btn']")));

        // ── API: Student token → POST /api/students → 403 ────────────────
        using var http = new HttpClient { BaseAddress = new Uri(ApiUrl) };

        // Get a Student JWT from the real API
        var loginBody = new StringContent(
            """{"username":"elakkia","password":"elakkia123!"}""",
            Encoding.UTF8, "application/json");
        var loginResp = await http.PostAsync("/api/auth/login", loginBody);
        loginResp.EnsureSuccessStatusCode();

        var json  = await loginResp.Content.ReadAsStringAsync();
        var token = JsonDocument.Parse(json).RootElement.GetProperty("token").GetString()!;

        http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var writeBody = new StringContent(
            """{"name":"HackAttempt","age":20,"rollNumber":"HACK1","email":"h@hack.com"}""",
            Encoding.UTF8, "application/json");
        var writeResp = await http.PostAsync("/api/students", writeBody);

        Assert.Equal(HttpStatusCode.Forbidden, writeResp.StatusCode);
    }
}
