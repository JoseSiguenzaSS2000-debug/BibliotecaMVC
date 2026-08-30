using BibliotecaMVC.Models;

namespace BibliotecaMVC.Repositories
{
    public interface IRepositorioLibro
    {
        IEnumerable<Libro> ObtenerTodos();
    }
}
