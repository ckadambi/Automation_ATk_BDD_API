using OpenQA.Selenium;
using Common; // <- pulls TestConfig.UiBaseUrl

namespace UiTests.Pages.SwagLabs
{
    public sealed class LoginPage : UiTests.Pages.BasePage
    {
        private static readonly By Username = By.Id("user-name");
        private static readonly By Password = By.Id("password");
        private static readonly By LoginButton = By.Id("login-button");
        private static readonly By Error = By.CssSelector("[data-test='error']");

        public LoginPage(IWebDriver driver) : base(driver) { }

        /// <summary>
        /// Navigates to the Swag Labs login page using the environment-specific base URL.
        /// Reads from Common/TestConfig.UiBaseUrl which is driven by config.json and TEST_ENVIRONMENT.
        /// Optional 'path' lets you append a relative path if your envs use subpaths (e.g., "/auth/login").
        /// </summary>
        // public void GoTo(string? path = null)
        // {
        //     var baseUrl = EnsureTrailingSlash(TestConfig.UiBaseUrl);
        //     var target = path is null ? baseUrl : Combine(baseUrl, path);
        //     Driver.Navigate().GoToUrl(target);

        //     // Wait until the page is ready (simple readyState check)
        //     ((IJavaScriptExecutor)Driver).ExecuteScript("return document.readyState");
        // }

        public void GoTo(string? path = null)
        {
            // Try to read Common/.env placed in the test output folder structure
            // var envFilePath = Path.Combine(AppContext.BaseDirectory, "Common", ".env");
            var envFilePath = Path.Combine(AppContext.BaseDirectory, ".env");

            string? uiBaseUrl = null;

            if (File.Exists(envFilePath))
            {
                foreach (var raw in File.ReadAllLines(envFilePath))
                {
                    var line = raw?.Trim();
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

                    var eq = line.IndexOf('=');
                    if (eq <= 0) continue;

                    var key = line[..eq].Trim();
                    var value = line[(eq + 1)..].Trim();

                    if (key.Equals("TEST_ENVIRONMENT", StringComparison.OrdinalIgnoreCase))
                        Environment.SetEnvironmentVariable("TEST_ENVIRONMENT", value);

                    if (key.Equals("UI_BASE_URL", StringComparison.OrdinalIgnoreCase))
                        uiBaseUrl = value;
                }
            }

            // Fallback to config.json-driven value if .env didn't provide UI_BASE_URL
            uiBaseUrl ??= TestConfig.UiBaseUrl;

            var baseUrl = EnsureTrailingSlash(uiBaseUrl);
            var target = path is null ? baseUrl : Combine(baseUrl, path);

            Driver.Navigate().GoToUrl(target);

            // Wait until the page is ready (simple readyState check)
            ((IJavaScriptExecutor)Driver).ExecuteScript("return document.readyState");
        }


        public void Login(string username, string password)
        {
            Find(Username).Clear();
            Find(Username).SendKeys(username);
            Find(Password).Clear();
            Find(Password).SendKeys(password);
            Find(LoginButton).Click();
        }

        public string? ErrorText()
        {
            try { return Find(Error).Text; }
            catch { return null; }
        }

        private static string EnsureTrailingSlash(string url) =>
            string.IsNullOrWhiteSpace(url) ? url : (url.EndsWith("/") ? url : url + "/");

        private static string Combine(string baseUrl, string relative) =>
            string.IsNullOrWhiteSpace(relative) ? baseUrl : new Uri(new Uri(baseUrl), relative.TrimStart('/')).ToString();
    }
}
