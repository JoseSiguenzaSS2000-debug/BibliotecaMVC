using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        private static List<Libro> _libros = new List<Libro>()
        {
            new Libro
            {
                Id = 1,
                Titulo = "Clean Code",
                Autor = "Robert Martin",
                Categoria = "Programacion",
                Precio = 35.50,
                Disponible = true,
            },

            new Libro
            {
                Id = 2,
                Titulo = "100 años de soledad",
                Autor = "Gabriel Garcia Marquez",
                Categoria = "Literatura",
                Precio = 18.00,
                Disponible = false,
            }
        };

        public IActionResult Index()
        {
            ViewBag.Libros = _libros;
            return View();
        }

        public IActionResult Details(int id)
        {
            var libro = _libros.FirstOrDefault(x => x.Id == id);

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Libro libro)
        {
            if (!ModelState.IsValid)
            {
                return View(libro);
            }

            if (_libros.Any())
            {
                libro.Id = _libros.Max(x => x.Id) + 1;
            }
            else
            {
                libro.Id = 1;
            }

            _libros.Add(libro);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var libro = _libros.FirstOrDefault(x => x.Id == id);

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Libro libro)
        {
            if (!ModelState.IsValid)
            {
                return View(libro);
            }

            var libroEditar = _libros.FirstOrDefault(x => x.Id == libro.Id);

            if (libroEditar == null)
            {
                return NotFound();
            }

            libroEditar.Titulo = libro.Titulo;
            libroEditar.Autor = libro.Autor;
            libroEditar.Categoria = libro.Categoria;
            libroEditar.Precio = libro.Precio;
            libroEditar.Disponible = libro.Disponible;

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var libro = _libros.FirstOrDefault(x => x.Id == id);

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Libro libro)
        {
            var libroEliminar = _libros.FirstOrDefault(x => x.Id == libro.Id);

            if (libroEliminar == null)
            {
                return NotFound();
            }

            _libros.Remove(libroEliminar);

            return RedirectToAction(nameof(Index));
        }
    }
}