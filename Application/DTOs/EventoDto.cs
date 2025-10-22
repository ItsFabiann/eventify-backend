using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EventifyAPI.Application.DTOs
{
    public class EventoResponseDto
    {
        public int EventoId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public DateTime FechaEvento { get; set; }
        public string Ubicacion { get; set; } = string.Empty;
        public int Capacidad { get; set; }
        public int AsientosDisponibles { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string? Imagen { get; set; }
        public string EstadoEvento { get; set; } = "Activo";
        public DateTime? FechaModificacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string OrganizadorNombre { get; set; } = string.Empty;
        public string? TelefonoOrganizador { get; set; }
    }

    public class EventoCreateRequestDto
    {
        [Required, MaxLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        [Required]
        public DateTime FechaEvento { get; set; }

        [Required, MaxLength(200)]
        public string Ubicacion { get; set; } = string.Empty;

        [Required, Range(1, 100000)]
        public int Capacidad { get; set; }

        [Required, MaxLength(50)]
        public string Categoria { get; set; } = string.Empty;

        [Required, Range(0, 10000)]
        public decimal Precio { get; set; }

        [Range(1, int.MaxValue)]
        public int? OrganizadorId { get; set; }

        public IFormFile? ImagenFile { get; set; }

        [Url(ErrorMessage = "URL de imagen inválida")]
        [MaxLength(500)]
        public string? ImagenUrl { get; set; }

        [MaxLength(20)]
        public string? EstadoEvento { get; set; } = "Activo";
    }

    public class EventoUpdateRequestDto
    {
        [MaxLength(200)]
        public string? Titulo { get; set; }

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        public DateTime? FechaEvento { get; set; }

        [MaxLength(200)]
        public string? Ubicacion { get; set; }

        [Range(1, 100000)]
        public int? Capacidad { get; set; }

        [MaxLength(50)]
        public string? Categoria { get; set; }

        [Range(0, 10000)]
        public decimal? Precio { get; set; }

        public IFormFile? ImagenFile { get; set; }

        [Url(ErrorMessage = "URL de imagen inválida")]
        [MaxLength(500)]
        public string? ImagenUrl { get; set; }

        [MaxLength(20)]
        public string? EstadoEvento { get; set; }
    }
}