using System.ComponentModel.DataAnnotations;

namespace EventifyAPI.Application.DTOs
{
    public class UsuarioResponseDto
    {
        public int UsuarioId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string? Dni { get; set; }
        public string? Telefono { get; set; }
        public string Rol { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; } = true;
    }

    public class UsuarioCreateRequestDto
    {
        [Required(ErrorMessage = "Email requerido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password requerido")]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nombre completo requerido")]
        [MaxLength(100)]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rol requerido")]
        [MaxLength(20)]
        public string Rol { get; set; } = "Asistente";

        [Required(ErrorMessage = "DNI requerido para Organizador/Asistente")]
        [MaxLength(20)]
        public string? Dni { get; set; }

        [Phone(ErrorMessage = "Teléfono inválido")]
        [MaxLength(20)]
        public string? Telefono { get; set; }
    }

    public class UsuarioUpdateRequestDto
    {
        [MaxLength(100)]
        public string? NombreCompleto { get; set; }

        [MaxLength(20)]
        public string? Rol { get; set; }

        [MaxLength(20)]
        public string? Dni { get; set; }

        [Phone(ErrorMessage = "Teléfono inválido")]
        [MaxLength(20)]
        public string? Telefono { get; set; }

        public bool? Activo { get; set; }
    }
}
