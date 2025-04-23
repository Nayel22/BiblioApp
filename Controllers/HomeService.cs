using BiblioApp.Models;
using System.Text;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
        public async Task<int> ObtenerIdUsuarioPorCorreoAsync(string correo)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Usuario/por-correo?correo={Uri.EscapeDataString(correo)}");

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
}
