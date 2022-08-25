using Chess.Shared.Dtos;
using System.Net.Http.Json;
using System;
using System.Text.Json.Serialization;
using System.Text;
using Newtonsoft.Json;
using Chess.Shared.Communications;
using Chess.Shared;

namespace Chess.WebAsm.Client.Services
{
    public class HttpService
    {
        private HttpClient _httpClient = null;
        private HttpClient HttpClient => _httpClient ??= CreateDefaultHttpClient();

        private HttpClient CreateDefaultHttpClient()
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

            return client;
        }
        public void SetToken(string token)
        {
            HttpClient.DefaultRequestHeaders.Add(UserDto.HeaderTokenKey, token);
        }

        public async Task<TResponse> Post<TRequest, TResponse>(string url, TRequest request)
            where TResponse : Response, new()
        {
            var response = await HttpClient.PostAsJsonAsync(url, request);
            var result = await response.Content.ReadFromJsonAsync<TResponse>();
            return result ?? new TResponse()
            {
                ErrorMessage = $"Read from json async returned null",
            };
        }
        public async Task<TResponse> Get<TRequest, TResponse>(string url, TRequest request)
            where TResponse : Response, new()
        {
            string bodyContent = JsonConvert.SerializeObject(request);
            var message = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Content = new StringContent(bodyContent, Encoding.UTF8, "application/json")
            };

            var result = await HttpClient.SendAsync(message);
            var response = await result.Content.ReadFromJsonAsync<TResponse>();
            return response ?? new TResponse()
            {
                ErrorMessage = $"Read from json async returned null",
            };
        }
        public async Task<TResponse> Get<TResponse>(string url)
            where TResponse : Response, new()
        {
            var result = await HttpClient.GetFromJsonAsync<TResponse>(url);
            return result ?? new TResponse()
            {
                ErrorMessage = $"Read from json async returned null",
            };
        }
    }
}
