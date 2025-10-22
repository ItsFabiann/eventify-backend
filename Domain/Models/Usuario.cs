using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventifyAPI.Domain.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string NombreCompleto { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Dni { get; set; }

        [MaxLength(20)]
        public string? Telefono { get; set; }

        [Required]
        [MaxLength(20)]
        public string Rol { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public bool Activo { get; set; } = true;

        public virtual ICollection<Evento> EventosOrganizados { get; set; } = new List<Evento>();
        public virtual ICollection<Boleto> BoletosComprados { get; set; } = new List<Boleto>();
        public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
    }
}
