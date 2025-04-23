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
            var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            Console.WriteLine($"📦 Prestamo generado para usuario ID: {idUsuario}");

            if (idUsuario == null || idUsuario == 0)
                return Unauthorized();

            var prestamo = new Prestamo
            {
                IdUsuario = idUsuario.Value,
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

        public async Task<IActionResult> PrestamosPendientes()
        {
            var correoUsuario = HttpContext.Session.GetString("Usuario");
            if (string.IsNullOrEmpty(correoUsuario))
                return Unauthorized();

            var prestamos = await _prestamoService.ObtenerPrestamosPendientesPorCorreoAsync(correoUsuario);
            return View(prestamos);
        }

        [HttpPost]
        public async Task<IActionResult> DevolverLibro(int idPrestamo)
        {
            try
            {
                var exito = await _prestamoService.MarcarComoDevueltoAsync(idPrestamo);
                if (!exito)
                    return BadRequest("No se pudo devolver el libro.");

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
    }
}