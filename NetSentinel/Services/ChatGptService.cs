using System;
using System.Net.Http;
using System.Threading.Tasks;
using NetSentinel.Interfaces;

using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace NetSentinel.Services;

public class ChatGptService : IChatGptService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _endpoint = "chat/completions";

    public ChatGptService(IConfiguration configuration)
    {
        _httpClient = new HttpClient();
        _baseUrl = configuration["AppSettings:BaseUrl"] ?? throw new InvalidOperationException("AppSettings:BaseUrl is not configured.");
        var apiKey = configuration["AppSettings:ApiKey"] ?? throw new InvalidOperationException("AppSettings:ApiKey is not configured.");
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Informatics.ChatGpt");
    }

    public async Task<ChatGptResponse?> GenerateResponseAsync(ChatGptRequest request)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        var json = JsonSerializer.Serialize(request, options);
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
        string url = $"{_baseUrl}/{_endpoint}";
        using (var response = await _httpClient.PostAsync(url, content))
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}. Details: {errorContent}");
                return null;
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ChatGptResponse>(responseContent, options);
        }
    }
}
