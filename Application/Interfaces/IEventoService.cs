using EventifyAPI.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventifyAPI.Application.Interfaces
{
    public interface IEventoService
    {
        Task<IEnumerable<EventoResponseDto>> GetAllAsync();
        Task<IEnumerable<EventoResponseDto>> GetAllAsync(string? categoria = null, decimal? precioMin = null, decimal? precioMax = null, string? busqueda = null);
        Task<EventoResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<EventoResponseDto>> GetByOrganizadorIdAsync(int organizadorId);
        Task<EventoResponseDto> CreateAsync(EventoCreateRequestDto request);
        Task<EventoResponseDto> UpdateAsync(int id, EventoUpdateRequestDto request);
        Task<bool> DeleteAsync(int id);
        Task DesactivarEventosPorOrganizadorAsync(int organizadorId);
    }
}