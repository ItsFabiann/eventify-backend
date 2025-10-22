using EventifyAPI.Application.DTOs;
using EventifyAPI.Application.Interfaces;
using EventifyAPI.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EventifyAPI.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly IUsuarioService _usuarioService;

        public AuthService(IConfiguration configuration, IUsuarioService usuarioService)
        {
            _secretKey = configuration["JwtSettings:SecretKey"];
            _issuer = configuration["JwtSettings:Issuer"];
            _audience = configuration["JwtSettings:Audience"];
            _usuarioService = usuarioService;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var usuario = await _usuarioService.GetByEmailAsync(request.Email);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
                throw new UnauthorizedAccessException("Credenciales inválidas");

            var token = GenerateJwtToken(usuario);

            return new LoginResponseDto
            {
                Token = token,
                Expiry = DateTime.UtcNow.AddHours(24),
                Rol = usuario.Rol,
                UsuarioId = usuario.UsuarioId
            };
        }

        public string GenerateJwtToken(Usuario usuario)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
