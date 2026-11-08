
using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Authorization;
using BibliotecaMVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BibliotecaMVC.Controllers
{
    [Authorize]
    public class AutoresController : Controller
    {
        private readonly BibliotecaContext _context;
        private readonly ILogger<AutoresController> _logger;

        public AutoresController(BibliotecaContext context, ILogger<AutoresController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Autores
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Consultando el listado de Autores");

            try
            {
                var autores = await _context.Autores.ToListAsync();
                return View(autores);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Se produjo un error al consultar el listado de autores");
                throw;
            }

            
        }


        public async Task<IActionResult> Reporte()
        {
            var autores = await _context.Autores
                .AsNoTracking()
                .OrderBy(autor => autor.Nombre)
                .ToListAsync();

            return File(GenerarReporte(autores), "application/pdf");
        }

        public async Task<IActionResult> DescargarReporte()
        {
            var autores = await _context.Autores
                .AsNoTracking()
                .OrderBy(autor => autor.Nombre)
                .ToListAsync();

            return File(GenerarReporte(autores), "application/pdf", "reporte-autores.pdf");
        }
        private static byte[] GenerarReporte(IReadOnlyCollection<Autor> autores)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(document => document.Page(page =>
            {
                page.Size(612, 792, Unit.Point);
                page.Margin(42);
                page.PageColor(Colors.Grey.Lighten5);
                page.Header().Column(column =>
                {
                    column.Item().Text("BIBLIOTECA UNIVERSITARIA")
                        .FontSize(11)
                        .Bold()
                        .FontColor(Colors.Blue.Darken2);

                    column.Item().Text("Reporte de autores")
                        .FontSize(26)
                        .Bold()
                        .FontColor(Colors.Grey.Darken3);

                    column.Item().Text($"Listadi actualizado el {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .FontSize(10)
                        .FontColor(Colors.Grey.Darken1);

                    column.Item().PaddingTop(14).LineHorizontal(2).LineColor(Colors.Blue.Medium);
                        

                });
                page.Content().PaddingVertical(22).Column(column =>
                {
                    column.Item().Background(Colors.Blue.Darken2).Padding(14).Row(row =>
                    {
                        row.RelativeItem().Text("AutoresRegistrados")
                        .FontSize(11)
                        .Bold()
                        .FontColor(Colors.White);
                        row.ConstantItem(45).AlignRight().Text(autores.Count.ToString())
                        .FontSize(18)
                        .Bold()
                        .FontColor(Colors.White);
                    });

                    column.Item().PaddingTop(18).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(42);
                            columns.RelativeColumn(2.5f);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1.3f);

                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCellStyle).Text("ID");
                            header.Cell().Element(HeaderCellStyle).Text("NOMBRE");
                            header.Cell().Element(HeaderCellStyle).Text("NACIONALIDAD");
                            header.Cell().Element(HeaderCellStyle).Text("NACIMIENTO");
                        });

                        foreach ( var autor in autores)
                        {
                            table.Cell().Element(BodyCellStyle).Text(autor.Id.ToString());
                            table.Cell().Element(BodyCellStyle).Text(autor.Nombre);
                            table.Cell().Element(BodyCellStyle).Text(autor.Nacionalidad ?? "No especificada");
                            table.Cell().Element(BodyCellStyle).Text(autor.FechaNacimiento.ToString("dd/MM/yyyy"));
                        }
                    });

                });
                page.Footer().AlignCenter().Text(text => 
                {
                    text.Span("Biblioteca universitaria - pagina ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();

                });
            })).GeneratePdf();

            static IContainer HeaderCellStyle(IContainer container) => container
                .Background(Colors.Blue.Darken2)
                .PaddingVertical(9)
                .PaddingHorizontal(8)
                .DefaultTextStyle(text => text.FontSize(9).Bold().FontColor(Colors.White));

            static IContainer BodyCellStyle(IContainer container) => container
                .Background(Colors.White)
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2)
                .PaddingVertical(9)
                .PaddingHorizontal(8)
                .DefaultTextStyle(text => text.FontSize(9).Bold().FontColor(Colors.Grey.Darken3));
        }




        // Autores detalles
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Consultando los detalles del autor con identificador {AutorId}",id);

            try
            {
                var autor = await _context.Autores.FirstOrDefaultAsync(a => a.Id == id);

                if (autor == null)
                {
                    _logger.LogWarning("No se encontro el autor solicitado con el identificador {AutorId}", id);
                    return NotFound();
                }

                return View(autor);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar los detalles del autor con identificador {AutorId}", id);
                throw;
            }
        }

        // Autores creación (GET)
        [Authorize(Roles = "Admin, Bibliotecario")]
        public IActionResult Create()
        {
            _logger.LogInformation("Iniciando la operacion de creacion de un autor");
            return View();
        }

        // Autores creación (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Bibliotecario")]
        public async Task<IActionResult> Create(Autor autor)
        {
            _logger.LogInformation("Iniciando la operacion de creacion de un autor ");
            if (!ModelState.IsValid)
            {
                _logger.LogError("El autor enviado no cumple con los requerimientos minimos");
                return View(autor);
            }

            try
            {
                _context.Autores.Add(autor);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al crear el autor");
                throw;
            }

        }

        // Autores editar (GET)
        [Authorize(Roles = "Admin, Bibliotecario")]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("Iniciando la operacion de edicion del autor con identificador {AutorId}", id);

            try
            {
                var autor = await _context.Autores.FindAsync(id);

                if (autor == null)
                {
                    return NotFound();
                }

                return View(autor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar los detalles del autor  para su edicion con identificador {AutorId}", id);
                throw;
            }
           
        }

        // Autores editar (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Bibliotecario")]
        public async Task<IActionResult> Edit(int id, Autor autor)
        {
            _logger.LogInformation("Iniciando la edicion del autor con identificador {AutorId}", id);
            if (id != autor.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(autor);
            }

            try
            {
                var exists = await _context.Autores.AnyAsync(a => a.Id == id);

                if (!exists)
                {
                    _logger.LogWarning("No se encontro el autor solicitado con identificador {AutorId}", id);
                    return NotFound();
                }

                _context.Update(autor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al editar el autor con identificador {AutorId}", id);
                throw;
            }
        }

        // Autores borrar: solo Admin
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Iniciando la eliminacion del autor con identificador {AutorId}", id);
            try
            {
                var autor = await _context.Autores.FindAsync(id);

                if (autor == null)
                {
                    _logger.LogWarning("No se encontro el autor solicitado para eliminar con identificador {AutorId}", id);
                    return NotFound();
                }

                _context.Autores.Remove(autor);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al eliminar el autor con identificador {AutorId}", id);
                throw;
            }
        }
    }
}
