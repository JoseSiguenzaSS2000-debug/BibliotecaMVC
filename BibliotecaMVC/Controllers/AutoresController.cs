using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using BibliotecaMVC.Services;

namespace BibliotecaMVC.Controllers
{
    public class AutoresController : Controller
    {
        private readonly IAutorService _autorService;

        public AutoresController(IAutorService autorService)
        {
            _autorService = autorService;
        }

        public IActionResult Index()
        {
            var autores = _autorService.ObtenerTodos();

            return View(autores);
        }

        public IActionResult Details(int id)
        {
            var autor = _autorService.ObtenerPorId(id);

            if (autor == null)
            {
                return NotFound();
            }

            return View(autor);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Autor autor)
        {
            if (!ModelState.IsValid)
            {
                return View(autor);
            }

            _autorService.Agregar(autor);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var autor = _autorService.ObtenerPorId(id);

            if (autor == null)
            {
                return NotFound();
            }

            return View(autor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Autor autor)
        {
            if (!ModelState.IsValid)
            {
                return View(autor);
            }

            var autorEditar = _autorService.ObtenerPorId(autor.Id);

            if (autorEditar == null)
            {
                return NotFound();
            }

            _autorService.Editar(autor);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var autor = _autorService.ObtenerPorId(id);

            if (autor == null)
            {
                return NotFound();
            }

            return View(autor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Autor autor)
        {
            var autorEliminar = _autorService.ObtenerPorId(autor.Id);

            if (autorEliminar == null)
            {
                return NotFound();
            }

            _autorService.Eliminar(autor.Id);

            return RedirectToAction(nameof(Index));
        }
    }
}