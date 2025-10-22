using EventifyAPI.Application.Interfaces;
using EventifyAPI.Domain.Models;
using EventifyAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventifyAPI.Infrastructure.Repositories
{
    public class PagoRepository : IPagoRepository
    {
        private readonly EventifyDbContext _context;

        public PagoRepository(EventifyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Pago>> GetAllAsync()
        {
            return await _context.Pagos
                .Include(p => p.Boleto)
                .ThenInclude(b => b.Evento)
                .ToListAsync();
        }

        public async Task<Pago?> GetByIdAsync(int id)
        {
            return await _context.Pagos
                .Include(p => p.Boleto)
                .FirstOrDefaultAsync(p => p.PagoId == id);
        }

        public async Task<Pago> CreateAsync(Pago pago)
        {
            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();
            return pago;
        }

        public async Task<Pago> UpdateAsync(Pago pago)
        {
            _context.Entry(pago).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return pago;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pago = await GetByIdAsync(id);
            if (pago == null) return false;
            _context.Pagos.Remove(pago);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Pago?> GetByBoletoIdAsync(int boletoId)
        {
            return await _context.Pagos.FirstOrDefaultAsync(p => p.BoletoId == boletoId);
        }

        public async Task AddAsync(Pago pago)
        {
            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();
        }
    }
}
