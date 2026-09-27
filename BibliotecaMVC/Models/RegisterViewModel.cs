using System.ComponentModel.DataAnnotations;



namespace BibliotecaMVC.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [Display(Name ="Usuario")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electronico es obligatorio")]
        [EmailAddress(ErrorMessage ="Introduce un correo electronico valido")]
        [Display(Name = "Correo electronico")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contrasenia es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contrasenia")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La confirmacion de contrasenia es obligatoria")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage ="Contrasenias no coinciden")]
        [Display(Name = "Confirmaciond e contrasenia")]
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}
