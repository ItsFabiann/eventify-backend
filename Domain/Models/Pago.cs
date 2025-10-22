using System;
using System.ComponentModel.DataAnnotations;

namespace EventifyAPI.Domain.Models
{
    public class Pago
    {
        public int PagoId { get; set; }

        [Required]
        public int BoletoId { get; set; }

        [Required]
        public decimal Monto { get; set; }

        public decimal Comision { get; set; } = 0.00m;

        [Required]
        public string MetodoPago { get; set; } = string.Empty;

        public string? IdTransaccion { get; set; }

        [Required]
        public string Estado { get; set; } = "Pendiente";

        [Required]
        public DateTime FechaPago { get; set; } = DateTime.UtcNow;

        public DateTime? FechaModificacion { get; set; }

        public virtual Boleto Boleto { get; set; } = null!;
    }
}
