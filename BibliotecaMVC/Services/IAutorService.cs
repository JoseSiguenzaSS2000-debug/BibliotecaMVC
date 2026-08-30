using BibliotecaMVC.Models;

namespace BibliotecaMVC.Services
{
    public interface IAutorService
    {
        IEnumerable<Autor> ObtenerTodos();
        Autor ObtenerPorId(int id);
        void Agregar(Autor autor);
        void Editar(Autor autor);
        void Eliminar(int id);
    }
}