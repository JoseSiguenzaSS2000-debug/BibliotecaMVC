
// Importa el modelo Libro de la aplicación principal.
using BibliotecaMVC.Models;

// Define el espacio de nombres de los servicios.
namespace BibliotecaMVC.Services;

// Define las operaciones disponibles para el módulo Libros.
public interface ILibroService
{
    // Obtiene todos los libros registrados.
    Task<List<Libro>> ObtenerTodosAsync();

    // Busca un libro mediante su identificador.
    Task<Libro?> ObtenerPorIdAsync(int id);

    // Registra un nuevo libro.
    Task RegistrarAsync(Libro libro);

    // Actualiza un libro existente.
    Task<bool> ActualizarAsync(Libro libro);

    // Elimina un libro mediante su identificador.
    Task<bool> EliminarAsync(int id);
}
