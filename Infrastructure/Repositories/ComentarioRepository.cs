using EventifyAPI.Application.Interfaces;
using EventifyAPI.Domain.Models;
using EventifyAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventifyAPI.Infrastructure.Repositories
{
    public class ComentarioRepository : IComentarioRepository
    {
        private readonly EventifyDbContext _context;

        public ComentarioRepository(EventifyDbContext context)
        {
            _context = context;
        }

        public async Task<Comentario> CreateAsync(Comentario comentario)
        {
            _context.Comentarios.Add(comentario);
            await _context.SaveChangesAsync();
            return comentario;
        }

        public async Task<IEnumerable<Comentario>> GetAllAsync()
        {
            return await _context.Comentarios
                .Include(c => c.Usuario)
                .ToListAsync();
        }

        public async Task<Comentario> GetByIdAsync(int id)
        {
            return await _context.Comentarios.FindAsync(id);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var comentario = await GetByIdAsync(id);
            if (comentario == null) return false;
            comentario.Eliminado = true;
            _context.Comentarios.Update(comentario);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Comentario> UpdateAsync(Comentario comentario)
        {
            _context.Comentarios.Update(comentario);
            await _context.SaveChangesAsync();
            return comentario;
        }

        public async Task<IEnumerable<Comentario>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Comentarios
                .Where(c => c.UsuarioId == usuarioId && !c.Eliminado)
                .Include(c => c.Usuario)
                .OrderByDescending(c => c.FechaEnvio)
                .ToListAsync();
        }
    }
}
