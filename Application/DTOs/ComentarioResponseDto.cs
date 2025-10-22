namespace EventifyAPI.Application.DTOs
{
    public class ComentarioResponseDto
    {
        public int ComentarioId { get; set; }
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string RolUsuario { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public DateTime FechaEnvio { get; set; }
        public bool Eliminado { get; set; }
    }
}