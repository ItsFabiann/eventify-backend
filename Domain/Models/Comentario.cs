using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventifyAPI.Domain.Models
{
    [Table("Comentarios")]
    public class Comentario
    {
        public int ComentarioId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Mensaje { get; set; } = string.Empty;

        public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;

        public bool Eliminado { get; set; } = false;

        public virtual Usuario Usuario { get; set; } = null!;
    }
}