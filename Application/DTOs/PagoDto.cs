using System.ComponentModel.DataAnnotations;

namespace EventifyAPI.Application.DTOs
{
    public class PagoResponseDto
    {
        public int PagoId { get; set; }
        public int BoletoId { get; set; }
        public decimal Monto { get; set; }
        public decimal Comision { get; set; }
        public string MetodoPago { get; set; }
        public string IdTransaccion { get; set; }
        public string Estado { get; set; }
        public DateTime FechaPago { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string BoletoNumero { get; set; } = string.Empty;
    }

    public class PagoCreateRequestDto
    {
        [Required]
        public int BoletoId { get; set; }

        [Required]
        public decimal Monto { get; set; }

        [Required]
        public string MetodoPago { get; set; }

        public decimal Comision { get; set; } = 0.00m;
    }

    public class PagoUpdateRequestDto
    {
        public string? Estado { get; set; }
        public string? IdTransaccion { get; set; }
        public decimal? Comision { get; set; }
    }
    public class ProcesarPagoSimuladoRequestDto
    {
        [Required]
        public int EventoId { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public string MetodoPago { get; set; }

        [Required]
        public decimal Monto { get; set; }

        public string Telefono { get; set; }
        public string CodigoAprobacion { get; set; }
    }

    public class ResultadoPagoDto
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
    }
}