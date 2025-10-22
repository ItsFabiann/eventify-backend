using EventifyAPI.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventifyAPI.Application.Interfaces
{
    public interface IComentarioRepository
    {
        Task<Comentario> CreateAsync(Comentario comentario);
        Task<IEnumerable<Comentario>> GetAllAsync();
        Task<Comentario> GetByIdAsync(int id);
        Task<Comentario> UpdateAsync(Comentario comentario);
        Task<bool> EliminarAsync(int id);
    }
}