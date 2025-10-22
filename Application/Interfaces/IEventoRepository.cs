using EventifyAPI.Domain.Models;

namespace EventifyAPI.Application.Interfaces
{
    public interface IEventoRepository
    {
        Task<IEnumerable<Evento>> GetAllAsync();
        Task<Evento?> GetByIdAsync(int id);
        Task<Evento> CreateAsync(Evento evento);
        Task<Evento> UpdateAsync(Evento evento);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Evento>> GetByOrganizadorIdAsync(int organizadorId);
    }
}