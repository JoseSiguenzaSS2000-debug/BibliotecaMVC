
using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using BibliotecaMVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace BibliotecaMVC.Controllers
{
    [Authorize]
    public class LibrosController : Controller
    {
        private readonly BibliotecaContext _context;
        private readonly ILogger<LibrosController> _logger;

        public LibrosController(BibliotecaContext context, ILogger<LibrosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Libros
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Consultando el listado de Libros");

            try
            {
                var libros = await _context.Libros.ToListAsync();
                return View(libros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar el listado de libros");
                throw;
            }
        }

        // Libros detalles
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Consultando los detalles del libro con identificador {LibroId}", id);

            try
            {
                var libro = await _context.Libros.FirstOrDefaultAsync(l => l.Id == id);

                if (libro == null)
                {
                    _logger.LogWarning("No se encontro el libro solicitado con el identificador {LibroId}", id);
                    return NotFound();
                }

                return View(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar los detalles del libro con identificador {LibroId}", id);
                throw;
            }
        }

        // Libros creación (GET)
        [Authorize(Roles = "Admin, Bibliotecario")]
        public IActionResult Create()
        {
            _logger.LogInformation("Iniciando la operacion de creacion de un libro");
            return View();
        }

        // Libros creación (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Bibliotecario")]
        public async Task<IActionResult> Create(Libro libro)
        {
            _logger.LogInformation("Iniciando la operacion de creacion de un libro");

            if (!ModelState.IsValid)
            {
                _logger.LogError("El libro enviado no cumple con los requerimientos minimos");
                return View(libro);
            }

            try
            {
                _context.Libros.Add(libro);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al crear el libro");
                throw;
            }
        }

        // Libros editar (GET)
        [Authorize(Roles = "Admin, Bibliotecario")]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("Iniciando la operacion de edicion del libro con identificador {LibroId}", id);

            try
            {
                var libro = await _context.Libros.FindAsync(id);

                if (libro == null)
                {
                    _logger.LogWarning("No se encontro el libro solicitado con identificador {LibroId}", id);
                    return NotFound();
                }

                return View(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar los detalles del libro para su edicion con identificador {LibroId}", id);
                throw;
            }
        }

        // Libros editar (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Bibliotecario")]
        public async Task<IActionResult> Edit(int id, Libro libro)
        {
            _logger.LogInformation("Iniciando la edicion del libro con identificador {LibroId}", id);

            if (id != libro.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(libro);
            }

            try
            {
                var exists = await _context.Libros.AnyAsync(l => l.Id == id);

                if (!exists)
                {
                    _logger.LogWarning("No se encontro el libro solicitado con identificador {LibroId}", id);
                    return NotFound();
                }

                _context.Update(libro);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al editar el libro con identificador {LibroId}", id);
                throw;
            }
        }

        // Libros borrar
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Bibliotecario")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Iniciando la eliminacion del libro con identificador {LibroId}", id);

            try
            {
                var libro = await _context.Libros.FindAsync(id);

                if (libro == null)
                {
                    _logger.LogWarning("No se encontro el libro solicitado para eliminar con identificador {LibroId}", id);
                    return NotFound();
                }

                _context.Libros.Remove(libro);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al eliminar el libro con identificador {LibroId}", id);
                throw;
            }
        }
    }
}
