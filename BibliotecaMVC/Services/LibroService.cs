
// Importa el contexto de base de datos.
using BibliotecaMVC.Data;
// Importa el modelo Libro.
using BibliotecaMVC.Models;
// Importa Entity Framework Core.
using Microsoft.EntityFrameworkCore;

// Define el espacio de nombres de los servicios.
namespace BibliotecaMVC.Services;

// Implementa las operaciones del servicio de libros.
public class LibroService : ILibroService
{
    // Declara el contexto de base de datos.
    private readonly BibliotecaContext _context;

    // Constructor que recibe el contexto.
    public LibroService(BibliotecaContext context)
    {
        _context = context;
    }

    // Obtiene todos los libros registrados.
    public async Task<List<Libro>> ObtenerTodosAsync()
    {
        return await _context.Libros.ToListAsync();
    }

    // Busca un libro mediante su identificador.
    public async Task<Libro?> ObtenerPorIdAsync(int id)
    {
        return await _context.Libros.FirstOrDefaultAsync(l => l.Id == id);
    }

    // Registra un nuevo libro.
    public async Task RegistrarAsync(Libro libro)
    {
        _context.Libros.Add(libro);
        await _context.SaveChangesAsync();
    }

    // Actualiza un libro existente.
    public async Task<bool> ActualizarAsync(Libro libro)
    {
        var libroExistente = await _context.Libros.FindAsync(libro.Id);

        if (libroExistente == null)
        {
            return false;
        }

        _context.Entry(libroExistente).CurrentValues.SetValues(libro);

        await _context.SaveChangesAsync();

        return true;
    }

    // Elimina un libro mediante su identificador.
    public async Task<bool> EliminarAsync(int id)
    {
        var libro = await _context.Libros.FindAsync(id);

        if (libro == null)
        {
            return false;
        }

        _context.Libros.Remove(libro);
        await _context.SaveChangesAsync();

        return true;
    }
}
