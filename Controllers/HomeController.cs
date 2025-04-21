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
            //return View();
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);   
                }
                return View();
            }

            bool esValido = await _usuarioService.ValidarUsuarioAsync(loginViewModel);

            if (esValido)
            {
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

    }
}
