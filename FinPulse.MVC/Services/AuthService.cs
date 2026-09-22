using System.Text;
using System.Text.Json;
using FinPulse.DTOs;

namespace FinPulse.MVC.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    // O HttpClient é injetado e gerenciado automaticamente pelo IHttpClientFactory!
    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<string?> LoginAsync(LoginDTO loginDto)
    {
        var json = JsonSerializer.Serialize(loginDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/auth/login", content);

        if (!response.IsSuccessStatusCode)
            return null;

        var responseContent = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(responseContent);

        // Extrai o Token JWT retornado pela API
        if (document.RootElement.TryGetProperty("token", out var tokenProp))
        {
            return tokenProp.GetString();
        }

        return null;
    }

    public async Task<bool> RegisterAsync(RegisterDTO registerDto)
    {
        var json = JsonSerializer.Serialize(registerDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/auth/register", content);
        return response.IsSuccessStatusCode;
    }
}