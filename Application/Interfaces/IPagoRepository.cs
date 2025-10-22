using EventifyAPI.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventifyAPI.Application.Interfaces
{
    public interface IPagoRepository
    {
        Task<IEnumerable<Pago>> GetAllAsync();
        Task<Pago?> GetByIdAsync(int id);
        Task<Pago> CreateAsync(Pago pago);
        Task<Pago> UpdateAsync(Pago pago);
        Task<bool> DeleteAsync(int id);
        Task<Pago?> GetByBoletoIdAsync(int boletoId);
        Task AddAsync(Pago pago);
    }
}