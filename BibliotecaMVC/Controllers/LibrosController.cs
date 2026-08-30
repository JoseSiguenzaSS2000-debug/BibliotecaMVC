using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using BibliotecaMVC.Repositories;


namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        private readonly IRepositorioLibro _repositorio;

        public LibrosController(IRepositorioLibro repositorio)
        {
            _repositorio = repositorio;
        }

        public IActionResult Index()

        {
            var libros = _repositorio.ObtenerTodos();

            return View(libros);
        }

       
    }
}