using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using BibliotecaMVC.Data;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Controllers
{
    public class AutoresController : Controller
    {
        private readonly BibliotecaContext _context;

        public AutoresController(BibliotecaContext context)
        {
            _context = context;
        }

        //  Autores
        public async Task<IActionResult> Index()
        {
            var autores = await _context.Autores.ToListAsync();

            return View(autores);
        }

        //  Autores detalles
        public async Task<IActionResult> Details(int id)
        {
            var autor = await _context.Autores
                .FirstOrDefaultAsync(a => a.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            return View(autor);
        }

        // Autores creaci[on
        public IActionResult Create()
        {
            return View();
        }

        //Autores creacion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Autor autor)
        {
            if (!ModelState.IsValid)
            {
                return View(autor);
            }

            _context.Autores.Add(autor);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Autores editar
        public async Task<IActionResult> Edit(int id)
        {
            var autor = await _context.Autores.FindAsync(id);

            if (autor == null)
            {
                return NotFound();
            }

            return View(autor);
        }

        // Autores editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Autor autor)
        {
            if (id != autor.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(autor);
            }

            var exists = await _context.Autores.AnyAsync(a => a.Id == id);
            if (!exists)
            {
                return NotFound();
            }

            _context.Update(autor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Autores borrar

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Delete(int id)
        {
            var autor = await _context.Autores.FindAsync(id);

            if (autor == null)
            {
                return NotFound();
            }

            _context.Autores.Remove(autor);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}