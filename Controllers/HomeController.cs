using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Http;
using BiblioApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BiblioApp.Controllers
{ 
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HomeService _usuarioService;
        public HomeController(ILogger<HomeController> logger, HomeService usuarioService)
        {
            _logger = logger;
            _usuarioService = usuarioService;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool esValido = await _usuarioService.ValidarUsuarioAsync(loginViewModel);

            if (esValido)
            {
                var tipoUsuario = await _usuarioService.ObtenerTipoUsuarioAsync(loginViewModel.Correo);
                HttpContext.Session.SetString("TipoUsuario", tipoUsuario);
                HttpContext.Session.SetString("Usuario", loginViewModel.Correo);
                return RedirectToAction("Index", "Libro");
            }

            ModelState.AddModelError(string.Empty, "Correo o clave incorrectos.");
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        } 

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        } 
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistroViewModel registroViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool registrado = await _usuarioService.RegistrarUsuarioAsync(registroViewModel);

            if (registrado)
            {
                return RedirectToAction("Index"); // o redirige a Login si quieres
            }

            ModelState.AddModelError(string.Empty, "No se pudo registrar el usuario.");
            return View();
        }

    }
}
