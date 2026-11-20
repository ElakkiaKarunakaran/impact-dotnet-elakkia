using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace StudentApi.E2E.Tests;

/// <summary>
/// Base class for all E2E tests. Creates a headless Chrome browser per test
/// and provides shared helpers (Login, FillField, WaitFor).
///
/// Pre-requisites (run before this suite):
///   cd Week07/StudentApi  &&  dotnet run          → API on http://localhost:5259
///   cd Week08/student-portal  &&  npm run dev     → SPA on http://localhost:5173
/// </summary>
public abstract class SeleniumBase : IDisposable
{
    protected readonly IWebDriver Driver;
    protected readonly WebDriverWait Wait;

    protected const string AppUrl = "http://localhost:5173";
    protected const string ApiUrl = "http://localhost:5259";

    protected SeleniumBase()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless=new");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--window-size=1366,768");
        // Selenium Manager auto-downloads ChromeDriver if not on PATH
        Driver = new ChromeDriver(options);
        Wait   = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    /// <summary>Navigate to /login and submit credentials; waits for /students.</summary>
    protected void Login(string username, string password)
    {
        Driver.Navigate().GoToUrl($"{AppUrl}/login");
        Wait.Until(d => d.FindElement(By.Id("username")));

        Driver.FindElement(By.Id("username")).Clear();
        Driver.FindElement(By.Id("username")).SendKeys(username);
        Driver.FindElement(By.Id("password")).Clear();
        Driver.FindElement(By.Id("password")).SendKeys(password);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        Wait.Until(d => d.Url.Contains("/students"));
    }

    /// <summary>Set a date input value via JS (avoids locale/format issues).</summary>
    protected void SetDateField(string id, string isoDate)
        => ((IJavaScriptExecutor)Driver).ExecuteScript(
               "arguments[0].value = arguments[1];",
               Driver.FindElement(By.Id(id)), isoDate);

    /// <summary>Wait until an element matching the CSS selector is visible.</summary>
    protected IWebElement WaitFor(string cssSelector)
        => Wait.Until(d =>
        {
            var el = d.FindElements(By.CssSelector(cssSelector)).FirstOrDefault();
            return (el != null && el.Displayed) ? el : null;
        })!;

    public void Dispose() => Driver.Quit();
}
