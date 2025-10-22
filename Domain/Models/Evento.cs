using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventifyAPI.Domain.Models
{
    public class Evento
    {
        public int EventoId { get; set; }

        [Required]
        public string Titulo { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        [Required]
        public DateTime FechaEvento { get; set; }

        [Required]
        public string Ubicacion { get; set; } = string.Empty;

        [Required]
        public int Capacidad { get; set; }

        public int AsientosDisponibles { get; set; }

        [Required]
        public string Categoria { get; set; } = string.Empty;

        [Required]
        public decimal Precio { get; set; }

        public string? Imagen { get; set; }

        [Required]
        public string EstadoEvento { get; set; } = "Activo";

        public DateTime? FechaModificacion { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Required]
        public int OrganizadorId { get; set; }

        public virtual Usuario Organizador { get; set; } = null!;
        public virtual ICollection<Boleto> Boletos { get; set; } = new List<Boleto>();
    }
}
