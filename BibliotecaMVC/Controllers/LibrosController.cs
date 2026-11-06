
// Importa el controlador MVC.
using Microsoft.AspNetCore.Mvc;
// Importa el modelo Libro.
using BibliotecaMVC.Models;
// Importa la interfaz del servicio de libros.
using BibliotecaMVC.Services;
// Importa las herramientas de autorización.
using Microsoft.AspNetCore.Authorization;
// Importa las herramientas de logging.
using Microsoft.Extensions.Logging;

namespace BibliotecaMVC.Controllers
{
    [Authorize]
    public class LibrosController : Controller
    {
        // Declara la dependencia del servicio de libros.
        private readonly ILibroService _libroService;

        // Declara la dependencia del logger.
        private readonly ILogger<LibrosController> _logger;

        // Constructor que recibe el servicio y el logger.
        public LibrosController(
            ILibroService libroService,
            ILogger<LibrosController> logger)
        {
            _libroService = libroService;
            _logger = logger;
        }

        // Libros
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Consultando el listado de Libros");

            try
            {
                // Obtiene los libros mediante el servicio.
                var libros = await _libroService.ObtenerTodosAsync();

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
            _logger.LogInformation(
                "Consultando los detalles del libro con identificador {LibroId}", id);

            try
            {
                // Busca el libro mediante el servicio.
                var libro = await _libroService.ObtenerPorIdAsync(id);

                if (libro == null)
                {
                    _logger.LogWarning(
                        "No se encontro el libro solicitado con el identificador {LibroId}", id);

                    return NotFound();
                }

                return View(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Se produjo un error al consultar los detalles del libro con identificador {LibroId}",
                    id);

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
                _logger.LogError(
                    "El libro enviado no cumple con los requerimientos minimos");

                return View(libro);
            }

            try
            {
                // Registra el libro mediante el servicio.
                await _libroService.RegistrarAsync(libro);

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
            _logger.LogInformation(
                "Iniciando la operacion de edicion del libro con identificador {LibroId}", id);

            try
            {
                // Busca el libro mediante el servicio.
                var libro = await _libroService.ObtenerPorIdAsync(id);

                if (libro == null)
                {
                    _logger.LogWarning(
                        "No se encontro el libro solicitado con identificador {LibroId}", id);

                    return NotFound();
                }

                return View(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Se produjo un error al consultar los detalles del libro para su edicion con identificador {LibroId}",
                    id);

                throw;
            }
        }

        // Libros editar (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Bibliotecario")]
        public async Task<IActionResult> Edit(int id, Libro libro)
        {
            _logger.LogInformation(
                "Iniciando la edicion del libro con identificador {LibroId}", id);

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
                // Actualiza el libro mediante el servicio.
                var actualizado = await _libroService.ActualizarAsync(libro);

                if (!actualizado)
                {
                    _logger.LogWarning(
                        "No se encontro el libro solicitado con identificador {LibroId}", id);

                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Se produjo un error al editar el libro con identificador {LibroId}",
                    id);

                throw;
            }
        }

        // Libros borrar
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Bibliotecario")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation(
                "Iniciando la eliminacion del libro con identificador {LibroId}", id);

            try
            {
                // Elimina el libro mediante el servicio.
                var eliminado = await _libroService.EliminarAsync(id);

                if (!eliminado)
                {
                    _logger.LogWarning(
                        "No se encontro el libro solicitado para eliminar con identificador {LibroId}",
                        id);

                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Se produjo un error al eliminar el libro con identificador {LibroId}",
                    id);

                throw;
            }
        }
    }
}
