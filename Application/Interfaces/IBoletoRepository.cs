using EventifyAPI.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventifyAPI.Application.Interfaces
{
    public interface IBoletoRepository
    {
        Task<IEnumerable<Boleto>> GetAllAsync();
        Task<Boleto?> GetByIdAsync(int id);
        Task<Boleto> CreateAsync(Boleto boleto);
        Task<Boleto> UpdateAsync(Boleto boleto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Boleto>> GetByEventoIdAsync(int eventoId);
        Task<IEnumerable<Boleto>> GetByUsuarioIdAsync(int usuarioId);
        Task AddAsync(Boleto boleto);
        int GetNextId();
    }
}
