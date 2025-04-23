using BiblioApp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace BiblioApp.Controllers
{
    public class PrestamoController : Controller
    {
        private readonly PrestamoService _prestamoService;

        public PrestamoController(PrestamoService prestamoService)
        {
            _prestamoService = prestamoService;
        }
        [HttpPost]
        public async Task<IActionResult> GenerarPrestamo(int idLibro, DateTime fechaDevolucionEsperada)
        {
            var correoUsuario = HttpContext.Session.GetString("Usuario");
            if (string.IsNullOrEmpty(correoUsuario))
                return Unauthorized();

            var idUsuario = await _prestamoService.ObtenerIdUsuarioPorCorreoAsync(correoUsuario);
            if (idUsuario == 0)
                return NotFound("Usuario no encontrado");

            var prestamo = new Prestamo
            {
                IdUsuario = idUsuario,
                IdLibro = idLibro,
                FechaPrestamo = DateTime.Now,
                FechaDevolucionEsperada = fechaDevolucionEsperada,
                FechaDevolucionReal = null,
                Estado = "Activo"
            };

            if (prestamo.FechaDevolucionEsperada < DateTime.Today)
            {
                return BadRequest(new { error = "La fecha de devolución esperada debe ser válida." });
            }

            try
            {
                await _prestamoService.CrearPrestamoAsync(prestamo);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }




        
    }
}