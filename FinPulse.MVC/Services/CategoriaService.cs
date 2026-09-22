using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FinPulse.DTOs;

namespace FinPulse.MVC.Services;

public class CategoriaService : ICategoriaService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public CategoriaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    private void AdicionarTokenBearer(string token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<IEnumerable<CategoriaDTO>?> ObterTodasAsync(string token)
    {
        AdicionarTokenBearer(token);

        var response = await _httpClient.GetAsync("api/v1/categorias");
        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IEnumerable<CategoriaDTO>>(content, _jsonOptions);
    }

    public async Task<CategoriaDTO?> ObterPorIdAsync(int id, string token)
    {
        AdicionarTokenBearer(token);

        var response = await _httpClient.GetAsync($"api/v1/categorias/{id}");
        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<CategoriaDTO>(content, _jsonOptions);
    }

    public async Task<CategoriaDTO?> CriarAsync(CategoriaCreateDTO categoriaDto, string token)
    {
        AdicionarTokenBearer(token);

        var json = JsonSerializer.Serialize(categoriaDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/v1/categorias", content);
        if (!response.IsSuccessStatusCode)
            return null;

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<CategoriaDTO>(responseContent, _jsonOptions);
    }

    public async Task<bool> DeletarAsync(int id, string token)
    {
        AdicionarTokenBearer(token);

        var response = await _httpClient.DeleteAsync($"api/v1/categorias/{id}");
        return response.IsSuccessStatusCode;
    }
}