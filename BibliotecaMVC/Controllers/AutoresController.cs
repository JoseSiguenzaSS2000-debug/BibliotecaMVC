using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Controllers
{
    public class AutoresController : Controller
    {
        private static List<Autor> _autores = new List<Autor>
            {
                new Autor
                {
                    Id = 1,
                    Nombre = "Gabriel García Márquez",
                    Nacionalidad = "Colombiana",
                    FechaNacimiento = new DateTime(1927, 3, 6)
                },

                new Autor
                {
                    Id = 2,
                    Nombre = "Isabel Allende",
                    Nacionalidad = "Chilena",
                    FechaNacimiento = new DateTime(1942, 8, 2)
                },

                new Autor
                {
                    Id = 3,
                    Nombre = "Salarrué",
                    Nacionalidad = "Salvadoreña",
                    FechaNacimiento = new DateTime(1899, 10, 22)
                },

                new Autor
                {
                    Id = 4,
                    Nombre = "Claudia Lars",
                    Nacionalidad = "Salvadoreña",
                    FechaNacimiento = new DateTime(1899, 12, 20)
                },

                new Autor
                {
                    Id = 5,
                    Nombre = "Roque Dalton",
                    Nacionalidad = "Salvadoreña",
                    FechaNacimiento = new DateTime(1935, 5, 14)
                }
            };

        public IActionResult Index()
        {
            return View(_autores);
        }

        public IActionResult Details(int id)
        {
            var autores = _autores.FirstOrDefault(x => x.Id == id);
            if (autores == null)
            {
                return NotFound();
            }

            return View(autores);
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

            if (_autores.Any())
            {
                autor.Id = _autores.Max(x => x.Id) + 1;
            }
            else
            {
                autor.Id = 1;
            }

            _autores.Add(autor);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)

        {
            var autor = _autores.FirstOrDefault(x => x.Id == id);

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
            var autorEditar = _autores.FirstOrDefault(x => x.Id == autor.Id);

            if (autorEditar == null)
            {
                return NotFound();
            }

            autorEditar.Nombre = autor.Nombre;
            autorEditar.Nacionalidad = autor.Nacionalidad;
            autorEditar.FechaNacimiento = autor.FechaNacimiento;

            return RedirectToAction(nameof(Index));



        }

        public IActionResult Delete(int id)
        {
            var autor = _autores.FirstOrDefault(x => x.Id == id);

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
            var autorEliminar = _autores.FirstOrDefault(x => x.Id == autor.Id);

            if (autorEliminar == null)
            {
                return NotFound();
            }

            _autores.Remove(autorEliminar);

            return RedirectToAction(nameof(Index));
        }




    }
}