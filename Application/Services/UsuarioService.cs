using AutoMapper;
using BCrypt.Net;
using EventifyAPI.Application.DTOs;
using EventifyAPI.Application.Interfaces;
using EventifyAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventifyAPI.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;
        private readonly IMapper _mapper;
        private readonly IEventoService _eventoService;

        public UsuarioService(IUsuarioRepository repository, IMapper mapper, IEventoService eventoService)
        {
            _repository = repository;
            _mapper = mapper;
            _eventoService = eventoService;
        }

        public async Task<IEnumerable<UsuarioResponseDto>> GetAllAsync()
        {
            var usuarios = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<UsuarioResponseDto>>(usuarios);
        }

        public async Task<UsuarioResponseDto?> GetByIdAsync(int id)
        {
            var usuario = await _repository.GetByIdAsync(id);
            return usuario == null ? null : _mapper.Map<UsuarioResponseDto>(usuario);
        }

        public async Task<UsuarioResponseDto> CreateAsync(UsuarioCreateRequestDto request)
        {
            if (await _repository.GetByEmailAsync(request.Email) != null)
                throw new InvalidOperationException("El email ya está registrado");

            if (request.Rol != "Admin" && string.IsNullOrWhiteSpace(request.Dni))
                throw new InvalidOperationException("DNI es requerido para rol Organizador o Asistente");

            if (!string.IsNullOrWhiteSpace(request.Dni) && request.Dni.Length < 8)
                throw new InvalidOperationException("DNI debe tener al menos 8 caracteres");

            var usuario = _mapper.Map<Usuario>(request);
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            usuario.FechaCreacion = DateTime.UtcNow;
            usuario.Activo = true;

            var created = await _repository.CreateAsync(usuario);
            return _mapper.Map<UsuarioResponseDto>(created);
        }

        public async Task<UsuarioResponseDto> UpdateAsync(int id, UsuarioUpdateRequestDto request)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException("Usuario no encontrado");

            if (string.IsNullOrWhiteSpace(request.NombreCompleto))
                throw new InvalidOperationException("NombreCompleto no puede estar vacío");

            if (string.IsNullOrWhiteSpace(request.Telefono))
                request.Telefono = null;

            existing.NombreCompleto = request.NombreCompleto;
            existing.Telefono = request.Telefono;

            var updated = await _repository.UpdateAsync(existing);
            return _mapper.Map<UsuarioResponseDto>(updated);
        }

        public async Task<bool> UpdateActivoAsync(int id, bool activo)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            if (!activo && existing.Rol == "Organizador")
            {
                var eventos = await _eventoService.GetByOrganizadorIdAsync(id);
                if (eventos.Any())
                    throw new InvalidOperationException("No se puede desactivar organizador con eventos registrados. Desactiva los eventos primero.");
            }

            existing.Activo = activo;
            await _repository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _repository.GetByEmailAsync(email);
        }
    }
}
