using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models
{
    public class Libro
    {
        public int Id { get; set; }
        public string Titulo { get; set; }

        [Required]
        [StringLength(100)]
        public string Autor { get; set; }

        public string Categoria { get; set; }
        public double Precio { get; set; }
        public bool Disponible { get; set; }
    }
}
