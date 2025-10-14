using TechTalk.SpecFlow;

namespace ApiTests.Hooks
{
    [Binding]
    public sealed class EnvHooks
    {
        private readonly ScenarioContext _ctx;

        public EnvHooks(ScenarioContext ctx) => _ctx = ctx;

        [BeforeScenario(Order = 0)]
        public void ApplyEnvironmentFromTag()
        {
            // Look for a tag like @env:dev, @env:qa, @env:stage, @env:prod
            var tag = _ctx.ScenarioInfo.Tags
                .FirstOrDefault(t => t.StartsWith("env:", StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(tag))
            {
                var env = tag.Split(':', 2).Last().Trim();
                // This is what TestConfig checks first
                Environment.SetEnvironmentVariable("TEST_ENVIRONMENT", env);
                Console.WriteLine($"[ApiTests] Using TEST_ENVIRONMENT={env}");
            }
        }
    }
}
