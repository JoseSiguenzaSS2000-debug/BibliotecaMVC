using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models
{
    public class LoginViewModel

    {
        [Required(ErrorMessage = "El usuario o correo electronico es obligatorio")]
        [Display(Name = "Usuario o correo electronico")]
        public string UserNameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contrasenia es obligatoria")]
        [DataType(DataType.Password)]
        public string Password {  get; set; } = string.Empty;

        [Display(Name = "Reccordarme ")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; } 
    }
}
