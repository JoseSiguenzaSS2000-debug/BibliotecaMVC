
using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Authorization;
using BibliotecaMVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BibliotecaMVC.Controllers
{
    [Authorize]
    public class CategoriasController : Controller
    {
        private readonly BibliotecaContext _context;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(BibliotecaContext context, ILogger<CategoriasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Categorías
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Consultando el listado de Categorías");

            try
            {
                var categorias = await _context.Categorias.ToListAsync();
                return View(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar el listado de categorías");
                throw;
            }
        }

        // Categorías creación (GET)
        [Authorize(Roles = "Admin, Bibliotecario")]
        public IActionResult Create()
        {
            _logger.LogInformation("Iniciando la operacion de creacion de una categoría");
            return View();
        }

        // Categorías creación (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Bibliotecario")]
        public async Task<IActionResult> Create(Categoria categoria)
        {
            _logger.LogInformation("Iniciando la operacion de creacion de una categoría");

            if (!ModelState.IsValid)
            {
                _logger.LogError("La categoría enviada no cumple con los requerimientos minimos");
                return View(categoria);
            }

            try
            {
                _context.Categorias.Add(categoria);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Categoría guardada correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al crear la categoría");
                throw;
            }
        }

        // Categorías editar (GET)
        [Authorize(Roles = "Admin, Bibliotecario")]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("Iniciando la operacion de edicion de la categoría con identificador {CategoriaId}", id);

            try
            {
                var categoria = await _context.Categorias.FindAsync(id);

                if (categoria == null)
                {
                    _logger.LogWarning("No se encontro la categoría solicitada con identificador {CategoriaId}", id);
                    return NotFound();
                }

                return View(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar los detalles de la categoría para su edicion con identificador {CategoriaId}", id);
                throw;
            }
        }

        // Categorías editar (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Bibliotecario")]
        public async Task<IActionResult> Edit(int id, Categoria categoria)
        {
            _logger.LogInformation("Iniciando la edicion de la categoría con identificador {CategoriaId}", id);

            if (id != categoria.ID)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            try
            {
                var exists = await _context.Categorias.AnyAsync(c => c.ID == id);

                if (!exists)
                {
                    _logger.LogWarning("No se encontro la categoría solicitada con identificador {CategoriaId}", id);
                    return NotFound();
                }

                _context.Categorias.Update(categoria);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Categoría actualizada correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al editar la categoría con identificador {CategoriaId}", id);
                throw;
            }
        }

        // Categorías borrar (GET)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Consultando la categoría para su eliminación con identificador {CategoriaId}", id);

            try
            {
                var categoria = await _context.Categorias.FindAsync(id);

                if (categoria == null)
                {
                    _logger.LogWarning("No se encontro la categoría solicitada para eliminar con identificador {CategoriaId}", id);
                    return NotFound();
                }

                return View(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al consultar la categoría para su eliminación con identificador {CategoriaId}", id);
                throw;
            }
        }

        // Categorías borrar (POST): solo Admin
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePost(int id)
        {
            _logger.LogInformation("Iniciando la eliminacion de la categoría con identificador {CategoriaId}", id);

            try
            {
                var categoria = await _context.Categorias.FindAsync(id);

                if (categoria == null)
                {
                    _logger.LogWarning("No se encontro la categoría solicitada para eliminar con identificador {CategoriaId}", id);
                    return NotFound();
                }

                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Categoría eliminada correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error al eliminar la categoría con identificador {CategoriaId}", id);
                throw;
            }
        }
    }
}
