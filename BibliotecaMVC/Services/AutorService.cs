using BibliotecaMVC.Models;

namespace BibliotecaMVC.Services
{
    public class AutorService : IAutorService
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

        public IEnumerable<Autor> ObtenerTodos()
        {
            return _autores;
        }

        public Autor ObtenerPorId(int id)
        {
            return _autores.FirstOrDefault(x => x.Id == id);
        }

        public void Agregar(Autor autor)
        {
            if (_autores.Any())
            {
                autor.Id = _autores.Max(x => x.Id) + 1;
            }
            else
            {
                autor.Id = 1;
            }

            _autores.Add(autor);
        }

        public void Editar(Autor autor)
        {
            var autorEditar = _autores.FirstOrDefault(x => x.Id == autor.Id);

            if (autorEditar != null)
            {
                autorEditar.Nombre = autor.Nombre;
                autorEditar.Nacionalidad = autor.Nacionalidad;
                autorEditar.FechaNacimiento = autor.FechaNacimiento;
            }
        }

        public void Eliminar(int id)
        {
            var autorEliminar = _autores.FirstOrDefault(x => x.Id == id);

            if (autorEliminar != null)
            {
                _autores.Remove(autorEliminar);
            }
        }
    }
}