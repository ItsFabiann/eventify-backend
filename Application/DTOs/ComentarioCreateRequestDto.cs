using System.ComponentModel.DataAnnotations;

namespace EventifyAPI.Application.DTOs
{
    public class ComentarioCreateRequestDto
    {
        [Required(ErrorMessage = "Mensaje requerido")]
        [MaxLength(1000, ErrorMessage = "Mensaje demasiado largo (máx 1000 chars)")]
        public string Mensaje { get; set; } = string.Empty;
    }
}