using BiblioApp.Models;
using System.Text;
using System.Text.Json;
namespace BiblioApp.Controllers
{
    public class HomeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public HomeService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"];

        }

        public async Task<bool> ValidarUsuarioAsync(LoginViewModel credenciales)
        {
            var url = $"{_baseUrl}/Usuario/validar?correo={Uri.EscapeDataString(credenciales.Correo)}&clave={Uri.EscapeDataString(credenciales.Clave)}";

            var response = await _httpClient.PostAsync(url, null);

            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RegistrarUsuarioAsync(RegistroViewModel usuario)
        {
            var url = $"{_baseUrl}/Usuario/registrar";
            var json = JsonSerializer.Serialize(usuario);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
            }

            return response.IsSuccessStatusCode;
        }


    }
}
