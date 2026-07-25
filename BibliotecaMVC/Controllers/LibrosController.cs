    using Microsoft.AspNetCore.Mvc;
    using BibliotecaMVC.Models;

    namespace BibliotecaMVC.Controllers
    {
        public class LibrosController : Controller
        {
            public IActionResult Index()
            {
                List<Libro> libros = new List<Libro>()
                {
                    new Libro
                    {
                        ID = 1,
                        Titulo = "Clean Code",
                        Autor = "Robert Martin",
                        Categoria = "Programacion",
                        Precio = 35.5,
                        Disponible = true,
                    },

                    new Libro {

                        ID = 2,
                        Titulo = "100 anios de soledad",
                        Autor = "Gabriel Garcia Marquez",
                        Categoria = "Literatura",
                        Precio = 18,
                        Disponible = false,


                    }
                };
            
                ViewBag.Libros = libros;

                return View();
            }
        }
    }
