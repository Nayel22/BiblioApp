using System.Text;
using BiblioApp.Models;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

public class PrestamoService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public PrestamoService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _baseUrl = configuration["ApiSettings:BaseUrl"];
    }

    public async Task CrearPrestamoAsync(Prestamo prestamo)
    {
        var json = JsonConvert.SerializeObject(prestamo);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/Prestamo", content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error al crear préstamo: {error}");
        }
    }

    public async Task<int> ObtenerIdUsuarioPorCorreoAsync(string correo)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/Usuario?correo={Uri.EscapeDataString(correo)}");

        if (!response.IsSuccessStatusCode)
        {
            return 0;
        }

        var content = await response.Content.ReadAsStringAsync();
        var usuarios = JsonConvert.DeserializeObject<List<Usuario>>(content);

        var usuario = usuarios?.FirstOrDefault();
        return usuario?.Id ?? 0;
    }


}