using System.ComponentModel.DataAnnotations;

namespace EventifyAPI.Application.DTOs
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Email requerido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password requerido")]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiry { get; set; }
        public string Rol { get; set; } = string.Empty;
        public int? UsuarioId { get; set; }
    }
}