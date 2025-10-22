using EventifyAPI.Application.DTOs;
using EventifyAPI.Domain.Models;

namespace EventifyAPI.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);

        string GenerateJwtToken(Usuario usuario);
    }
}