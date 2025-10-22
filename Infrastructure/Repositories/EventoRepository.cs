using EventifyAPI.Application.Interfaces;
using EventifyAPI.Domain.Models;
using EventifyAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventifyAPI.Infrastructure.Repositories
{
    public class EventoRepository : IEventoRepository
    {
        private readonly EventifyDbContext _context;

        public EventoRepository(EventifyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Evento>> GetAllAsync()
        {
            return await _context.Eventos
                .Include(e => e.Organizador)
                .ToListAsync();
        }

        public async Task<Evento?> GetByIdAsync(int id)
        {
            return await _context.Eventos
                .Include(e => e.Organizador)
                .Include(e => e.Boletos)
                .FirstOrDefaultAsync(e => e.EventoId == id);
        }

        public async Task<Evento> CreateAsync(Evento evento)
        {
            _context.Eventos.Add(evento);
            await _context.SaveChangesAsync();
            return evento;
        }

        public async Task<Evento> UpdateAsync(Evento evento)
        {
            _context.Entry(evento).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return evento;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var evento = await GetByIdAsync(id);
            if (evento == null) return false;
            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Evento>> GetByOrganizadorIdAsync(int organizadorId)
        {
            return await _context.Eventos
                .Where(e => e.OrganizadorId == organizadorId)
                .ToListAsync();
        }
    }
}
