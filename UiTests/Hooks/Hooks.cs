using BoDi;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using Common;
using Common.Drivers;
using OpenQA.Selenium.Support.Extensions;

namespace UiTests.Hooks
{
    [Binding]
    public sealed class Hooks
    {
        private readonly IObjectContainer _container;
        private readonly ScenarioContext _scenarioContext;
        private IWebDriver? _driver;

        public Hooks(IObjectContainer container, ScenarioContext scenarioContext)
        {
            _container = container;
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario(Order = 0)]
        public void BeforeScenario()
        {
            // Prefer env var, fall back to tag, then default from TestConfig
            var browserFromEnv = Environment.GetEnvironmentVariable("BROWSER");
            var browserFromTag = _scenarioContext.ScenarioInfo.Tags
                .FirstOrDefault(t => t.StartsWith("browser:", StringComparison.OrdinalIgnoreCase))
                ?.Split(':').Last();

            var chosen = !string.IsNullOrWhiteSpace(browserFromEnv)
                ? browserFromEnv
                : (browserFromTag ?? TestConfig.Browser);

            _driver = BrowserFactory.Create(chosen, TestConfig.Headless);
            _container.RegisterInstanceAs<IWebDriver>(_driver);

            Console.WriteLine($"[UiTests] Browser={chosen}, Headless={TestConfig.Headless}");
        }        
        
        [AfterScenario(Order = -1000)] // run BEFORE quitting the driver
        public void TakeScreenshotOnFailure()
        {
            try
            {
                if (_driver == null) return;

                var failed = _scenarioContext.TestError != null || _scenarioContext.ScenarioExecutionStatus != ScenarioExecutionStatus.OK;
                if (!failed) return;

                var file = SaveScreenshot(_driver, _scenarioContext.ScenarioInfo.Title, _scenarioContext.ScenarioInfo.Tags);
                Console.WriteLine($"[UiTests] Saved failure screenshot: {file}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UiTests] Screenshot capture failed: {ex}");
            }
        }

        [AfterScenario(Order = 10000)]
        public void AfterScenario()
        {
            try { _driver?.Quit(); _driver?.Dispose(); } catch { }
        }

        private static string SaveScreenshot(IWebDriver driver, string title, string[] tags)
        {
            var baseDir = Path.Combine(AppContext.BaseDirectory, "TestResults", "Screenshots");
            Directory.CreateDirectory(baseDir);

            string Sanitize(string s) => string.Join("_", s.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries))
                                              .Replace(' ', '_');
            var tagPart = tags.Any() ? $"[{Sanitize(string.Join('-', tags))}]" : "[]";
            var name = $"{DateTime.UtcNow:yyyyMMdd_HHmmssfff}_{Sanitize(title)}_{tagPart}.png";

            var path = Path.Combine(baseDir, name);
            var png = driver.TakeScreenshot();
            png.SaveAsFile(path);
            return path;
        }
    }
}