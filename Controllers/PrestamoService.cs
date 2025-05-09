﻿using System.Text;
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

  

    public async Task<List<Prestamo>> ObtenerPrestamosPendientesAsync(string correo)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/Prestamo/pendientes?correo={Uri.EscapeDataString(correo)}");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error al obtener préstamos pendientes: {error}");
        }

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<Prestamo>>(content);
    }

    public async Task<bool> MarcarComoDevueltoAsync(int idPrestamo)
    {
        var response = await _httpClient.PutAsync($"{_baseUrl}/Prestamo/devolver/{idPrestamo}", null);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error al devolver préstamo: {error}");
        }

        return true;
    }
    public async Task<List<Prestamo>> ObtenerPrestamosPendientesPorCorreoAsync(string correo)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/Prestamo/pendientes?correo={Uri.EscapeDataString(correo)}");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error al obtener préstamos pendientes: {error}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var prestamos = JsonConvert.DeserializeObject<List<Prestamo>>(content);
        return prestamos ?? new List<Prestamo>();
    }

    public async Task<string> ObtenerTipoUsuarioAsync(string correo)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/Usuario/tipo?correo={Uri.EscapeDataString(correo)}");

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("No se pudo obtener el tipo de usuario.");
        }

        var tipo = await response.Content.ReadAsStringAsync();
        return tipo.Trim('"');
    }




}