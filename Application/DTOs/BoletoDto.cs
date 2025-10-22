using System.ComponentModel.DataAnnotations;

namespace EventifyAPI.Application.DTOs
{
    public class BoletoResponseDto
    {
        public int BoletoId { get; set; }
        public int EventoId { get; set; }
        public int UsuarioId { get; set; }
        public string TipoBoleto { get; set; } = "General";
        public string NumeroBoleto { get; set; } = string.Empty;
        public int Cantidad { get; set; } = 1;
        public decimal Precio { get; set; }
        public string? CodigoQR { get; set; }
        public string Estado { get; set; } = "Disponible";
        public DateTime FechaCompra { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string EventoTitulo { get; set; } = string.Empty;
        public string UsuarioEmail { get; set; } = string.Empty;
    }

    public class BoletoCreateRequestDto
    {
        [Required, Range(1, int.MaxValue)]
        public int EventoId { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int UsuarioId { get; set; }

        [Required, MaxLength(50)]
        public string TipoBoleto { get; set; } = "General";

        [MaxLength(50)]
        public string? NumeroBoleto { get; set; }

        [Required, Range(1, 10)]
        public int Cantidad { get; set; } = 1;

        [MaxLength(255)]
        public string? CodigoQR { get; set; }
    }

    public class BoletoUpdateRequestDto
    {
        [MaxLength(20)]
        public string? Estado { get; set; }

        [MaxLength(255)]
        public string? CodigoQR { get; set; }

        [MaxLength(50)]
        public string? NumeroBoleto { get; set; }
    }

    public class CompraBoletoRequestDto
    {
        [Required, Range(1, int.MaxValue)]
        public int EventoId { get; set; }

        [Required, MaxLength(50)]
        public string TipoBoleto { get; set; } = "General";

        [Required, Range(1, 10)]
        public int Cantidad { get; set; } = 1;

        [Required, MaxLength(50)]
        public string MetodoPago { get; set; }
    }
}