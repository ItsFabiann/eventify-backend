using System.Collections.Generic;
using System.Threading.Tasks;
using EventifyAPI.Application.DTOs;

namespace EventifyAPI.Application.Interfaces
{
    public interface IBoletoService
    {
        Task<IEnumerable<BoletoResponseDto>> GetAllAsync();
        Task<BoletoResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<BoletoResponseDto>> CreateMultipleAsync(BoletoCreateRequestDto request);
        Task<BoletoResponseDto> UpdateAsync(int id, BoletoUpdateRequestDto request);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<BoletoResponseDto>> GetByUsuarioIdAsync(int usuarioId);
    }
}