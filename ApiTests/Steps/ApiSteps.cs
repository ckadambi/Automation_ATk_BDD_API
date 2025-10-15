using System.Text.Json;
using FluentAssertions;
using RestSharp;
using TechTalk.SpecFlow;
using Common;        // TestConfig
using Common.Api;   // ApiClient

namespace ApiTests.Steps
{
    [Binding]
    public class ApiSteps
    {
        private string _baseUrl = "";
        private RestResponse? _response;
        private ApiClient? _api;

        // If you want to inspect tags later, you can inject ScenarioContext here too.
        public ApiSteps() { }


        [Given(@"the API base url is default")]
        public void GivenTheApiBaseUrlIsDefault()
        {
            // 🔹 Dynamically load from TestConfig (.env or config.json)
            var env = TestConfig.EnvironmentName;
            _baseUrl = TestConfig.ApiBaseUrl;

            // 🔹 Log for clarity
            Console.WriteLine($"[ApiTests] Using API Base URL: {_baseUrl} (Environment={env})");

            // 🔹 Initialize RestSharp client
            _api = new ApiClient(_baseUrl);
        }

        [When(@"I GET ""(.*)""")]
        public async Task WhenIGet(string path)
        {
            _api.Should().NotBeNull("API client should be initialized");
            _response = await _api!.GetAsync(path);
        }

        [Then(@"the response status should be (.*)")]
        public void ThenTheResponseStatusShouldBe(int expectedStatusCode)
        {
            _response.Should().NotBeNull();
            ((int)_response!.StatusCode).Should().Be(expectedStatusCode);
        }

        [Then(@"the response should contain JSON with ""(.*)"" and ""(.*)"" and ""(.*)""")]
        public void ThenTheResponseShouldContainJsonWith(string key1, string key2, string key3)
        {
            _response.Should().NotBeNull();
            _response!.Content.Should().NotBeNullOrWhiteSpace("we expect a JSON body");
            using var doc = JsonDocument.Parse(_response.Content!);
            var root = doc.RootElement;
            root.TryGetProperty(key1, out _).Should().BeTrue($"response should contain '{key1}'");
            root.TryGetProperty(key2, out _).Should().BeTrue($"response should contain '{key2}'");
            root.TryGetProperty(key3, out _).Should().BeTrue($"response should contain '{key3}'");
        }
    }
}
