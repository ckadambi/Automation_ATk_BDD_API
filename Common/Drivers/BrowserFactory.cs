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
