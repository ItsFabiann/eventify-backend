using EventifyAPI.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventifyAPI.Application.Interfaces
{
    public interface IPagoService
    {
        Task<IEnumerable<PagoResponseDto>> GetAllAsync();
        Task<PagoResponseDto?> GetByIdAsync(int id);
        Task<PagoResponseDto> CreateAsync(PagoCreateRequestDto request);
        Task<PagoResponseDto> UpdateAsync(int id, PagoUpdateRequestDto request);
        Task<bool> DeleteAsync(int id);
        Task<ResultadoPagoDto> ProcesarPagoSimuladoAsync(int eventoId, int usuarioId, int cantidad, string metodoPago, decimal monto, string telefono, string codigoAprobacion);
    }
}