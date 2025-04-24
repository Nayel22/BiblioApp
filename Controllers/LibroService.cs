using BiblioApp.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace BiblioApp.Controllers
{
    public class LibroService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public LibroService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<List<Libro>> GetAllLibrosAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Libro");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al obtener libros: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Libro>>(json);
        }

       public async Task<Libro> GetLibroByIdAsync(int id)
{
    var response = await _httpClient.GetAsync($"{_baseUrl}/Libro/{id}");

    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
    {
        return null;
    }

    response.EnsureSuccessStatusCode();

    var content = await response.Content.ReadAsStringAsync();
    return JsonConvert.DeserializeObject<Libro>(content);
}


        public async Task<Libro> CreateLibroAsync(Libro libro)
        {
            var content = new StringContent(JsonConvert.SerializeObject(libro), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/Libro", content);
            response.EnsureSuccessStatusCode();

            return libro;
        }

        public async Task<Libro> UpdateLibroAsync(int id, Libro libro)
        {
            var content = new StringContent(JsonConvert.SerializeObject(libro), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl}/Libro/{id}", content);
            response.EnsureSuccessStatusCode();

            return libro;
        }

        public async Task<bool> DeleteLibroAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/Libro/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
