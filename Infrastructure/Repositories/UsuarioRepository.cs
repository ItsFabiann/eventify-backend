using EventifyAPI.Application.Interfaces;
using EventifyAPI.Domain.Models;
using EventifyAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventifyAPI.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly EventifyDbContext _context;

        public UsuarioRepository(EventifyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == id);
        }

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<Usuario> UpdateAsync(Usuario usuario)
        {
            _context.Update(usuario);
            try
            {
                _context.Database.ExecuteSqlRaw("ALTER TABLE Usuarios NOCHECK CONSTRAINT ALL;");
                await _context.SaveChangesAsync();
                _context.Database.ExecuteSqlRaw("ALTER TABLE Usuarios CHECK CONSTRAINT ALL;");
                Console.WriteLine($"✅ SaveChanges exitoso para Usuario ID {usuario.UsuarioId}");
                return usuario;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en Repository UpdateAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"❌ Inner Exception: {ex.InnerException.Message}");
                    Console.WriteLine($"❌ Stack Trace: {ex.StackTrace}");
                }
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var usuario = await GetByIdAsync(id);
            if (usuario == null) return false;
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
