using BiblioApp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using OfficeOpenXml;

namespace BiblioApp.Controllers
{
    public class PrestamoController : Controller
    {
        private readonly PrestamoService _prestamoService;
        private readonly LibroService _libroService;

        public PrestamoController(PrestamoService prestamoService, LibroService libroService)
        {
            _prestamoService = prestamoService;
            _libroService = libroService;
        }

        // GET: /Prestamo
        public async Task<IActionResult> Index()
        {
            var prestamos = await _prestamoService.GetAllPrestamosAsync();
            return View(prestamos);
        }

        // GET: /Prestamo/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var prestamo = await _prestamoService.GetPrestamoByIdAsync(id);
            if (prestamo == null)
            {
                return NotFound();
            }
            return View(prestamo);
        }

        // GET: /Prestamo/Create
        public async Task<IActionResult> Create()
        {
            // Obtener lista de libros para el dropdown
            ViewBag.Libros = await _libroService.GetAllLibrosAsync();
            return View();
        }

        // POST: /Prestamo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Prestamo prestamo)
        {
            if (ModelState.IsValid)
            {
                // Establecer fecha actual como fecha de préstamo
                prestamo.FechaPrestamo = DateTime.Now;
                // Establecer estado inicial
                prestamo.Estado = "Pendiente";

                await _prestamoService.CreatePrestamoAsync(prestamo);
                return RedirectToAction(nameof(Index));
            }

            // Si hay errores, volver a cargar los libros para el dropdown
            ViewBag.Libros = await _libroService.GetAllLibrosAsync();
            return View(prestamo);
        }

        // GET: /Prestamo/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var prestamo = await _prestamoService.GetPrestamoByIdAsync(id);
            if (prestamo == null)
            {
                return NotFound();
            }

            ViewBag.Libros = await _libroService.GetAllLibrosAsync();
            return View(prestamo);
        }

        // POST: /Prestamo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Prestamo prestamo)
        {
            if (id != prestamo.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _prestamoService.UpdatePrestamoAsync(id, prestamo);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Libros = await _libroService.GetAllLibrosAsync();
            return View(prestamo);
        }

        // GET: /Prestamo/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var prestamo = await _prestamoService.GetPrestamoByIdAsync(id);
            if (prestamo == null)
            {
                return NotFound();
            }
            return View(prestamo);
        }

        // POST: /Prestamo/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _prestamoService.DeletePrestamoAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Prestamo/DevolucionLibro/5
        public async Task<IActionResult> DevolucionLibro(int id)
        {
            var prestamo = await _prestamoService.GetPrestamoByIdAsync(id);
            if (prestamo == null)
            {
                return NotFound();
            }
            return View(prestamo);
        }

        // POST: /Prestamo/DevolucionLibro/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DevolucionLibroConfirmada(int id)
        {
            var prestamo = await _prestamoService.GetPrestamoByIdAsync(id);
            if (prestamo == null)
            {
                return NotFound();
            }

            // Actualizar la devolución
            prestamo.FechaDevolucionReal = DateTime.Now;
            prestamo.Estado = "Devuelto";

            await _prestamoService.UpdatePrestamoAsync(id, prestamo);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ExportToPdf()
        {
            var prestamos = await _prestamoService.GetAllPrestamosAsync();

            using (var stream = new MemoryStream())
            {
                var doc = new iTextSharp.text.Document();
                PdfWriter.GetInstance(doc, stream).CloseStream = false;
                doc.Open();

                doc.Add(new iTextSharp.text.Paragraph("Lista de Préstamos"));
                doc.Add(new iTextSharp.text.Paragraph(" "));

                var table = new PdfPTable(6);
                table.AddCell("ID");
                table.AddCell("Usuario");
                table.AddCell("Libro");
                table.AddCell("Fecha Préstamo");
                table.AddCell("Fecha Devolución Esperada");
                table.AddCell("Estado");

                foreach (var prestamo in prestamos)
                {
                    table.AddCell(prestamo.Id.ToString());
                    table.AddCell(prestamo.Usuario?.Nombre ?? $"ID: {prestamo.IdUsuario}");
                    table.AddCell(prestamo.Libro?.Titulo ?? $"ID: {prestamo.IdLibro}");
                    table.AddCell(prestamo.FechaPrestamo.ToShortDateString());
                    table.AddCell(prestamo.FechaDevolucionEsperada.ToShortDateString());
                    table.AddCell(prestamo.Estado);
                }

                doc.Add(table);
                doc.Close();

                stream.Position = 0;
                return File(stream.ToArray(), "application/pdf", "Prestamos.pdf");
            }
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var prestamos = await _prestamoService.GetAllPrestamosAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Préstamos");

                // Encabezados
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Usuario";
                worksheet.Cells[1, 3].Value = "Libro";
                worksheet.Cells[1, 4].Value = "Fecha Préstamo";
                worksheet.Cells[1, 5].Value = "Fecha Devolución Esperada";
                worksheet.Cells[1, 6].Value = "Fecha Devolución Real";
                worksheet.Cells[1, 7].Value = "Estado";

                int row = 2;
                foreach (var prestamo in prestamos)
                {
                    worksheet.Cells[row, 1].Value = prestamo.Id;
                    worksheet.Cells[row, 2].Value = prestamo.Usuario?.Nombre ?? $"ID: {prestamo.IdUsuario}";
                    worksheet.Cells[row, 3].Value = prestamo.Libro?.Titulo ?? $"ID: {prestamo.IdLibro}";
                    worksheet.Cells[row, 4].Value = prestamo.FechaPrestamo;
                    worksheet.Cells[row, 5].Value = prestamo.FechaDevolucionEsperada;
                    worksheet.Cells[row, 6].Value = prestamo.FechaDevolucionReal;
                    worksheet.Cells[row, 7].Value = prestamo.Estado;
                    row++;
                }

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Prestamos.xlsx");
            }
        }
    }
}