using EventifyAPI.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventifyAPI.Application.Interfaces
{
    public interface IComentarioService
    {
        Task<ComentarioResponseDto> CrearAsync(string mensaje, int usuarioId);
        Task<IEnumerable<ComentarioResponseDto>> GetAllAsync();
        Task<bool> EliminarAsync(int id);
    }
}