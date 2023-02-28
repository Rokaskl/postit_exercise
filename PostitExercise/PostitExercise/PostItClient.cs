using System.Text.Json;
using System.Web;

namespace PostitExercise
{
    public interface IPostItClient
    {
        public Task<string> GetZipCodeFromAdderessAsync(string address);
    }

    public class PostItClient : IPostItClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey;
        private readonly string _baseUri;

        public PostItClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiKey = configuration.GetValue<string>("PostIt:ApiKey");
            _baseUri = configuration.GetValue<string>("PostIt:BaseUri");
        }

        public async Task<string> GetZipCodeFromAdderessAsync(string address)
        {
            var httpClient = _httpClientFactory.CreateClient();

            string url = $"{_baseUri}/?term={HttpUtility.UrlEncode(address)}&key={_apiKey}";

            using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            
            //response.data[0].post_code
            var resultParsed = JsonSerializer.Deserialize<PostItResponse>(result);
            return resultParsed.data?.FirstOrDefault()?.post_code ?? string.Empty;

        }
    }
}