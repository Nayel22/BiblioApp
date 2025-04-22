using BiblioApp.Models;
using Newtonsoft.Json;
using System.Text;

namespace BiblioApp.Controllers
{
    public class PrestamoService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public PrestamoService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<IEnumerable<Prestamo>> GetAllPrestamosAsync()
        {
            var response = await _httpClient.GetAsync(_baseUrl + "/Prestamo");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<Prestamo>>(content);
        }

        public async Task<Prestamo> GetPrestamoByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl + "/Prestamo"}/{id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Prestamo>(content);
        } 

        public async Task<Prestamo> CreatePrestamoAsync(Prestamo prestamo)
        {
            var content = new StringContent(JsonConvert.SerializeObject(prestamo), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_baseUrl + "/Prestamo", content);
            response.EnsureSuccessStatusCode();
            return prestamo;
        }

        public async Task<Prestamo> UpdatePrestamoAsync(int id, Prestamo prestamo)
        {
            var content = new StringContent(JsonConvert.SerializeObject(prestamo), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl + "/Prestamo"}/{id}", content);
            response.EnsureSuccessStatusCode();
            return prestamo;
        }

        public async Task<bool> DeletePrestamoAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl + "/Prestamo"}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
