using EventifyAPI.Application.Interfaces;
using EventifyAPI.Domain.Models;
using EventifyAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventifyAPI.Infrastructure.Repositories
{
    public class BoletoRepository : IBoletoRepository
    {
        private readonly EventifyDbContext _context;

        public BoletoRepository(EventifyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Boleto>> GetAllAsync()
        {
            return await _context.Boletos
                .Include(b => b.Evento)
                .Include(b => b.Usuario)
                .ToListAsync();
        }

        public async Task<Boleto?> GetByIdAsync(int id)
        {
            return await _context.Boletos
                .Include(b => b.Evento)
                .Include(b => b.Usuario)
                .FirstOrDefaultAsync(b => b.BoletoId == id);
        }

        public async Task<Boleto> CreateAsync(Boleto boleto)
        {
            _context.Boletos.Add(boleto);
            await _context.SaveChangesAsync();
            return boleto;
        }

        public async Task<Boleto> UpdateAsync(Boleto boleto)
        {
            _context.Entry(boleto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return boleto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var boleto = await GetByIdAsync(id);
            if (boleto == null) return false;
            _context.Boletos.Remove(boleto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Boleto>> GetByEventoIdAsync(int eventoId)
        {
            return await _context.Boletos
                .Where(b => b.EventoId == eventoId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Boleto>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Boletos
                .Include(b => b.Evento)
                .Include(b => b.Usuario)
                .Where(b => b.UsuarioId == usuarioId && (b.Evento == null || b.Evento.EstadoEvento == "Activo"))
                .ToListAsync();
        }

        public int GetNextId()
        {
            return _context.Boletos.Any() ? _context.Boletos.Max(b => b.BoletoId) + 1 : 1;
        }

        public async Task AddAsync(Boleto boleto)
        {
            _context.Boletos.Add(boleto);
            await _context.SaveChangesAsync();
        }
    }
}
