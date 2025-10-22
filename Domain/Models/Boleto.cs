using System;
using System.ComponentModel.DataAnnotations;

namespace EventifyAPI.Domain.Models
{
    public class Boleto
    {
        public int BoletoId { get; set; }

        [Required]
        public int EventoId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public string TipoBoleto { get; set; } = "General";

        public string NumeroBoleto { get; set; } = string.Empty;

        [Required]
        public int Cantidad { get; set; } = 1;

        [Required]
        public decimal Precio { get; set; }

        public string? CodigoQR { get; set; }

        [Required]
        public string Estado { get; set; } = "Disponible";

        [Required]
        public DateTime FechaCompra { get; set; } = DateTime.UtcNow;

        public DateTime? FechaModificacion { get; set; }

        public virtual Evento Evento { get; set; } = null!;
        public virtual Usuario Usuario { get; set; } = null!;
    }
}
