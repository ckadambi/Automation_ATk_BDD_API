using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace Common.Drivers
{
    public static class BrowserFactory
    {
        public static IWebDriver Create(string? browser, bool headless)
        {
            var b = (browser ?? "chrome").Trim().ToLowerInvariant();
            return b switch
            {
                "firefox" or "ff" => CreateFirefox(headless),
                _ => CreateChrome(headless),
            };
        }

        private static IWebDriver CreateChrome(bool headless)
        {
            var opts = new ChromeOptions();
            opts.AddUserProfilePreference("credentials_enable_service", false);
            opts.AddUserProfilePreference("profile.password_manager_enabled", false);
            opts.AddUserProfilePreference("autofill.profile_enabled", false);
            opts.AddUserProfilePreference("autofill.credit_card_enabled", false);
            // Newer Chrome (belt & suspenders):
            opts.AddUserProfilePreference("autofill.enabled", false);
            opts.AddUserProfilePreference("autofill.password_enabled", false);

            //  Run Incognito (Chrome does NOT offer to save passwords in incognito)
            opts.AddArgument("--incognito");
            // Disable related UI surfaces via features
            opts.AddArgument("--disable-features=PasswordManagerOnboarding,AutofillServerCommunication");

            if (headless)
            {
                // Headless mode — simulate a large screen
                opts.AddArgument("--headless=new");
                opts.AddArgument("--window-size=1920,1080");
            }
            else
            {
                // Headed mode — open maximized
                opts.AddArgument("--start-maximized");
                // optional: full-screen (kiosk)
                // opts.AddArgument("--kiosk");
            }

            // Stability flags
            opts.AddArgument("--no-sandbox");
            opts.AddArgument("--disable-dev-shm-usage");

            opts.PageLoadStrategy = PageLoadStrategy.Normal;

            var driver = new ChromeDriver(opts);

            // 🔧 Sometimes start-maximized doesn’t take effect on some OS/driver combos.
            // Explicitly maximize the window via WebDriver API.
            if (!headless)
            {
                driver.Manage().Window.Maximize();
            }

            return driver;
        }

        private static IWebDriver CreateFirefox(bool headless)
        {
            var opts = new FirefoxOptions();
            opts.SetPreference("signon.rememberSignons", false);
            opts.SetPreference("signon.autofillForms", false);
            opts.SetPreference("signon.passwordEditCapture.enabled", false);


            if (headless)
            {
                // Headless mode: emulate a large screen
                opts.AddArgument("-headless");
                opts.AddArgument("--width=1920");
                opts.AddArgument("--height=1080");
            }
            else
            {
                // Headed mode: start maximized
                opts.AddArgument("--start-maximized");
                // Optional: truly full-screen (like kiosk mode)
                // opts.AddArgument("--kiosk");
            }

            opts.PageLoadStrategy = PageLoadStrategy.Normal;

            var service = FirefoxDriverService.CreateDefaultService();
            var driver = new FirefoxDriver(service, opts, TimeSpan.FromSeconds(60));

            // 🔧 Fallback: Explicit maximize via WebDriver (works across OS)
            if (!headless)
            {
                driver.Manage().Window.Maximize();
            }

            return driver;
        }

    }
}
