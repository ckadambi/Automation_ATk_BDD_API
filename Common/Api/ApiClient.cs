using System.Text.Json;
using RestSharp;

namespace Common.Api
{
    public class ApiClient
    {
        private readonly RestClient _client;

        public ApiClient(string baseUrl)
        {
            _client = new RestClient(new RestClientOptions(baseUrl));
        }

        public async Task<RestResponse> GetAsync(string path) =>
            await _client.ExecuteAsync(new RestRequest(path, Method.Get));

        // T must be a reference type for RestSharp's AddJsonBody<T>
        public async Task<RestResponse> PostJsonAsync<T>(string path, T body) where T : class =>
            await _client.ExecuteAsync(
                new RestRequest(path, Method.Post).AddJsonBody(body)
            );

        public async Task<RestResponse> PutJsonAsync<T>(string path, T body) where T : class =>
            await _client.ExecuteAsync(
                new RestRequest(path, Method.Put).AddJsonBody(body)
            );

        public async Task<RestResponse> DeleteAsync(string path) =>
            await _client.ExecuteAsync(new RestRequest(path, Method.Delete));
    }
}
