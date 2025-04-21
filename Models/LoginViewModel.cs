using System.ComponentModel.DataAnnotations;

namespace BiblioApp.Models
{
    public class LoginViewModel
    {

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe ser un correo válido")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La clave es obligatoria")]
        public string Clave { get; set; }
    }
}
