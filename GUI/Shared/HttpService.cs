using System.Net;
using System.Text;

namespace GUI.Shared
{
    public class HttpService
    {
        private readonly HttpClient _client;

        public HttpService()
        {
            HttpClientHandler handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.All
            };

            _client = new HttpClient();
        }

        public async Task<string> GetAsync(string uri)
        {
            using HttpResponseMessage response = await _client.GetAsync(uri);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> PostAsync(string uri, string data, HttpMethod method, string contentType = "application/json")
        {
            using HttpContent content = new StringContent(data, Encoding.UTF8, contentType);

            HttpRequestMessage requestMessage = new HttpRequestMessage()
            {
                Content = content,
                Method = method,
                RequestUri = new Uri(uri)
            };

            using HttpResponseMessage response = await _client.SendAsync(requestMessage);

            return await response.Content.ReadAsStringAsync();
        }
    }
}
