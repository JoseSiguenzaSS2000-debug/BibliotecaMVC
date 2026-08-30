using BibliotecaMVC.Models;

namespace BibliotecaMVC.Repositories
{
    public class RepositorioEnMemoria : IRepositorioLibro
    {
        public IEnumerable<Libro> ObtenerTodos()

        {
            return new List<Libro>()
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
            },

            new Libro
            {
                Id = 3,
                Titulo = "Nuevo Libro",
                Autor = "Nuevo Autor",
                Categoria = "Nueva",
                Precio = 25.00,
                Disponible = false,
            }

            };
        }
    }
}
