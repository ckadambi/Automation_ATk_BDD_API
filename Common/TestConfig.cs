using System.Text.Json;

namespace Common
{
    public static class TestConfig
    {
        private static readonly Lazy<Dictionary<string, EnvironmentConfig>> _environments = new(LoadConfig);
        private static readonly Lazy<string> _activeEnvironment = new(GetActiveEnvironment);

        private static bool _envLoaded = false;
        private static readonly object _lock = new();

        // ---- Public surface ----
        public static string EnvironmentName
        {
            get { EnsureEnvLoaded(); return _activeEnvironment.Value; }
        }

        public static string UiBaseUrl
        {
            get { EnsureEnvLoaded(); return GetValue(env => env.UiBaseUrl, "https://www.saucedemo.com/"); }
        }

        public static string ApiBaseUrl
        {
            get { EnsureEnvLoaded(); return GetValue(env => env.ApiBaseUrl, "https://jsonplaceholder.typicode.com/"); }
        }

        public static bool Headless
        {
            get
            {
                EnsureEnvLoaded();
                return (Environment.GetEnvironmentVariable("HEADLESS") ?? "true")
                    .Equals("true", StringComparison.OrdinalIgnoreCase);
            }
        }

        public static string Browser
        {
            get
            {
                EnsureEnvLoaded();
                return (Environment.GetEnvironmentVariable("BROWSER") ?? "chrome").ToLowerInvariant();
            }
        }

        // ---- Impl ----
        private static void EnsureEnvLoaded()
        {
            if (_envLoaded) return;
            lock (_lock)
            {
                if (_envLoaded) return;

                // Look for .env in output directory first
                var p1 = Path.Combine(AppContext.BaseDirectory, ".env");
                // Optional fallback (only if you also copy Common/.env folder structure)
                var p2 = Path.Combine(AppContext.BaseDirectory, "Common", ".env");
                var path = File.Exists(p1) ? p1 : (File.Exists(p2) ? p2 : null);

                if (path != null)
                {
                    foreach (var raw in File.ReadAllLines(path))
                    {
                        var line = raw?.Trim();
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

                        var eq = line.IndexOf('=');
                        if (eq <= 0) continue;

                        var key = line[..eq].Trim();
                        var value = line[(eq + 1)..].Trim().Trim('"', '\'');

                        // Only set if not already set by the shell/CI (shell/env should win)
                        if (Environment.GetEnvironmentVariable(key) is null)
                        {
                            Environment.SetEnvironmentVariable(key, value);
                        }
                    }
                }

                _envLoaded = true;
            }
        }

        private static string GetActiveEnvironment()
        {
            var fromEnv = Environment.GetEnvironmentVariable("TEST_ENVIRONMENT");
            if (!string.IsNullOrWhiteSpace(fromEnv)) return fromEnv.ToLowerInvariant();

            var json = File.ReadAllText(ConfigPath);
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("environment").GetString() ?? "qa";
        }

        private static Dictionary<string, EnvironmentConfig> LoadConfig()
        {
            if (!File.Exists(ConfigPath))
                throw new FileNotFoundException($"Missing config file: {ConfigPath}");

            var json = File.ReadAllText(ConfigPath);
            using var doc = JsonDocument.Parse(json);

            var envs = new Dictionary<string, EnvironmentConfig>(StringComparer.OrdinalIgnoreCase);
            if (doc.RootElement.TryGetProperty("environments", out var envSection))
            {
                foreach (var env in envSection.EnumerateObject())
                {
                    var ui = env.Value.GetProperty("UiBaseUrl").GetString() ?? "";
                    var api = env.Value.GetProperty("ApiBaseUrl").GetString() ?? "";
                    envs[env.Name.ToLowerInvariant()] = new EnvironmentConfig
                    {
                        UiBaseUrl = ui,
                        ApiBaseUrl = api
                    };
                }
            }
            return envs;
        }

        private static string ConfigPath => Path.Combine(AppContext.BaseDirectory, "config.json");

        private static string GetValue(Func<EnvironmentConfig, string> selector, string fallback)
        {
            var envName = _activeEnvironment.Value;
            if (_environments.Value.TryGetValue(envName, out var env))
            {
                var value = selector(env);
                if (!string.IsNullOrWhiteSpace(value)) return value;
            }
            return fallback;
        }

        private class EnvironmentConfig
        {
            public string UiBaseUrl { get; set; } = "";
            public string ApiBaseUrl { get; set; } = "";
        }
    }
}
