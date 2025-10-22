using EventifyAPI.Application.DTOs;
using EventifyAPI.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventifyAPI.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioResponseDto>> GetAllAsync();
        Task<UsuarioResponseDto?> GetByIdAsync(int id);
        Task<UsuarioResponseDto> CreateAsync(UsuarioCreateRequestDto request);
        Task<UsuarioResponseDto> UpdateAsync(int id, UsuarioUpdateRequestDto request);
        Task<bool> UpdateActivoAsync(int id, bool activo);
        Task<bool> DeleteAsync(int id);
        Task<Usuario?> GetByEmailAsync(string email);
    }
}