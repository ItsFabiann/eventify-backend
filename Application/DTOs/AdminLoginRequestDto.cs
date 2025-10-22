using System.ComponentModel.DataAnnotations;

namespace EventifyAPI.Application.DTOs
{
    public class AdminLoginRequestDto
    {
        [Required(ErrorMessage = "Clave requerida")]
        [MinLength(1, ErrorMessage = "Clave inválida")]
        public string Clave { get; set; } = string.Empty;
    }
}