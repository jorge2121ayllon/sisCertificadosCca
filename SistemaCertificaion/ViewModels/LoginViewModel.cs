using System.ComponentModel.DataAnnotations;

namespace SistemaCertificaion.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        public bool Recordarme { get; set; }
    }
}
