# 🧩 AutomationSuite (.NET 8 – SpecFlow + xUnit + Selenium + RestSharp)

A unified automation framework supporting both **UI (Selenium)** and **API (RestSharp)** testing, built with **SpecFlow BDD** and **xUnit**, featuring dynamic environment configuration and CI/CD-ready pipelines.

---

## ⚙️ Solution Structure
```
AutomationSuite/
├── Common/        # Shared config, driver factory, API client, env loader
├── UiTests/       # Selenium + SpecFlow + xUnit UI tests
├── ApiTests/      # RestSharp + SpecFlow + xUnit API tests
├── .vscode/       # launch.json / tasks.json for VS Code
├── .github/       # GitHub Actions CI
├── azure-pipelines.yml
└── AutomationSuite.sln
```

---

## 📦 Packages Used

### Common
- `Selenium.WebDriver` — Core WebDriver API  
- `Selenium.Support` — Selenium helpers (waits, expected conditions)  
- `Selenium.WebDriver.ChromeDriver`, `Selenium.WebDriver.GeckoDriver` — Browser drivers  
- `RestSharp` — HTTP API client  
- `System.Text.Json` — JSON parsing for config  

### UiTests
- `SpecFlow`, `SpecFlow.xUnit`, `SpecFlow.Tools.MsBuild.Generation` — BDD engine  
- `xunit`, `xunit.runner.visualstudio` — Test framework + runner  
- `Microsoft.NET.Test.Sdk` — Integration with `dotnet test`  
- `FluentAssertions` — Readable assertions  

### ApiTests
- `RestSharp` — API testing  
- `SpecFlow`, `xUnit`, `FluentAssertions` — BDD + testing framework  
- `Microsoft.NET.Test.Sdk` — Test discovery  

---

## 🌍 Environment Management

### `config.json`
Defines base URLs for all environments:
```json
{
  "environment": "qa",
  "environments": {
    "dev": { "UiBaseUrl": "https://dev.saucedemo.com/", "ApiBaseUrl": "https://dev-api.example.com/" },
    "qa": { "UiBaseUrl": "https://qa.saucedemo.com/", "ApiBaseUrl": "https://qa-api.example.com/" },
    "prod": { "UiBaseUrl": "https://www.saucedemo.com/", "ApiBaseUrl": "https://api.example.com/" }
  }
}
```

### `.env` (in `Common/`)
Overrides or supplements environment values:
```bash
TEST_ENVIRONMENT=qa
BROWSER=chrome
HEADLESS=false
UI_BASE_URL=https://qa.saucedemo.com/
API_BASE_URL=https://qa-api.example.com/
```

### `Common/TestConfig`
Automatically loads `.env` and merges it with `config.json`.

---

## 🧠 Core Capabilities

### 🌐 UI Automation
- **Browsers:** Chrome & Firefox  
- **Headless:** Configurable (`HEADLESS=true/false`)  
- **Parallel:** Enabled via `xunit.runner.json`  
- **Page Object Model (POM):**
  - `BasePage` (shared waits/helpers)
  - `DuckDuckGoHomePage`, `SwagLabs.LoginPage`, `SwagLabs.InventoryPage`, `SwagLabs.CartPage`
- **Hooks.cs**
  - Loads environment & browser settings
  - Creates driver via `BrowserFactory`
  - Registers `IWebDriver` with SpecFlow container
  - Takes screenshots on failure

### 🔄 API Automation
- **Common.Api.ApiClient** wraps RestSharp:
  ```csharp
  await _api.GetAsync("/posts/1");
  await _api.PostJsonAsync("/users", new { name = "John" });
  ```
- **SpecFlow BDD steps** validate status codes & response structure.  
- Supports `@env:<name>` tags to switch environments per scenario.

---

## 🧪 Running Tests

### ▶ PowerShell CLI
```powershell
# Run UI tests on QA Chrome (headed)
$env:TEST_ENVIRONMENT="qa"
$env:BROWSER="chrome"
$env:HEADLESS="false"
dotnet test UiTests --filter "Category=realapp"

# Run API tests on QA
$env:TEST_ENVIRONMENT="qa"; dotnet test ApiTests
```

### ▶ VS Code Debugging
Use `launch.json`:
```json
"env": {
  "TEST_ENVIRONMENT": "qa",
  "BROWSER": "chrome",
  "HEADLESS": "false"
}
```

### ▶ Test Explorer
Create a `.env` file at workspace root or link `Common/.env` via VS Code settings.

---

## ⚙️ CI/CD Pipelines

### GitHub Actions (`.github/workflows/ci.yml`)
- Build & restore solution
- Run API tests
- Run UI tests on Chrome/Firefox matrix
- Upload screenshots & TRX results as artifacts

### Azure DevOps (`azure-pipelines.yml`)
- Stage 1: Build + API tests  
- Stage 2: UI tests (Chrome & Firefox)  
- Publishes screenshots and results  

---

## 🧩 Highlights

- Dynamic environment switching (`.env` + `config.json`)
- Multi-browser support (Chrome & Firefox)
- Parallel & headless execution
- Full POM structure for maintainability
- Screenshots on failure
- CI-ready GitHub Actions & Azure pipelines
- Tag filtering for selective test runs (`@realapp`, `@browser:chrome`, `@Jira-123`)

---

## 🚀 Quick Start

```powershell
dotnet restore
dotnet build
$env:TEST_ENVIRONMENT="qa"; $env:BROWSER="chrome"; dotnet test UiTests
```

---

**AutomationSuite** delivers a full-stack automation framework ready for local debug, CI/CD pipelines, and multi-environment testing.



Command to run test on specific env and browser
  - $env:TEST_ENVIRONMENT="qa"; $env:BROWSER="chrome"; dotnet test UiTests --filter "Category=Jira-123"

For headless(currently defaulted to false)-
  $env:TEST_ENVIRONMENT="qa"; $env:BROWSER="chrome"; $env:HEADLESS="false"; dotnet test UiTests --filter "Category=realapp"

in .env set as per env and browser-
TEST_ENVIRONMENT=qa
BROWSER=chrome/firefox
HEADLESS=false