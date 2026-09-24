using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using BibliotecaMVC.Data;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        private readonly BibliotecaContext _context;

        public LibrosController(BibliotecaContext context)
        {
            _context = context;
        }

        // Libros
        public async Task<IActionResult> Index()
        {
            var libros = await _context.Libros.ToListAsync();

            return View(libros);
        }

        // Libros detalles
        public async Task<IActionResult> Details(int id)
        {
            var libro = await _context.Libros
                .FirstOrDefaultAsync(l => l.Id == id);

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // Libros creación
        public IActionResult Create()
        {
            return View();
        }

        // Libros creación
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Libro libro)
        {
            if (!ModelState.IsValid)
            {
                return View(libro);
            }

            _context.Libros.Add(libro);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Libros editar
        public async Task<IActionResult> Edit(int id)
        {
            var libro = await _context.Libros.FindAsync(id);

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // Libros editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Libro libro)
        {
            if (id != libro.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(libro);
            }

            var exists = await _context.Libros.AnyAsync(l => l.Id == id);
            if (!exists)
            {
                return NotFound();
            }

            _context.Update(libro);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Libros borrar
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var libro = await _context.Libros.FindAsync(id);

            if (libro == null)
            {
                return NotFound();
            }

            _context.Libros.Remove(libro);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}