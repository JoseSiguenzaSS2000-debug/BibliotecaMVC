    using Microsoft.AspNetCore.Mvc;
    using BibliotecaMVC.Models;

    namespace BibliotecaMVC.Controllers
    {
        public class AutoresController : Controller
        {
            public IActionResult Index()
            {
                List<Autor> autores = new List<Autor>()
                {
                    new Autor
                    {
                        ID = 1,
                        Nombre = "Gabriel",
                        Apellido = "García Márquez",
                        Nacionalidad = "Colombiana",
                        FechaNacimiento = new DateTime(1927, 3, 6),
                        Activo = false
                    },

                    new Autor
                    {
                        ID = 2,
                        Nombre = "Isabel",
                        Apellido = "Allende",
                        Nacionalidad = "Chilena",
                        FechaNacimiento = new DateTime(1942, 8, 2),
                        Activo = true
                    },

                    new Autor
                    {
                        ID = 3,
                        Nombre = "Salarrué",
                        Apellido = "Salazar Arrué",
                        Nacionalidad = "Salvadoreña",
                        FechaNacimiento = new DateTime(1899, 10, 22),
                        Activo = false
                    },

                    new Autor
                    {
                        ID = 4,
                        Nombre = "Claudia",
                        Apellido = "Lars",
                        Nacionalidad = "Salvadoreña",
                        FechaNacimiento = new DateTime(1899, 12, 20),
                        Activo = false
                    },

                    new Autor
                    {
                        ID = 5,
                        Nombre = "Roque",
                        Apellido = "Dalton",
                        Nacionalidad = "Salvadoreña",
                        FechaNacimiento = new DateTime(1935, 5, 14),
                        Activo = false
                    }
                };

                ViewBag.Autores = autores;

                return View();
            }
        }
    }