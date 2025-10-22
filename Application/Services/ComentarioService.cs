using AutoMapper;
using EventifyAPI.Application.DTOs;
using EventifyAPI.Application.Interfaces;
using EventifyAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventifyAPI.Application.Services
{
    public class ComentarioService : IComentarioService
    {
        private readonly IComentarioRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUsuarioRepository _usuarioRepository;

        public ComentarioService(IComentarioRepository repository, IMapper mapper, IUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<ComentarioResponseDto> CrearAsync(string mensaje, int usuarioId)
        {
            var zonaPeru = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            var fechaPeru = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zonaPeru);

            var comentario = new Comentario
            {
                UsuarioId = usuarioId,
                Mensaje = mensaje,
                FechaEnvio = fechaPeru,
                Eliminado = false
            };

            var created = await _repository.CreateAsync(comentario);
            var user = await _usuarioRepository.GetByIdAsync(usuarioId);
            var response = _mapper.Map<ComentarioResponseDto>(created);
            response.NombreUsuario = user?.NombreCompleto ?? "Usuario Desconocido";
            response.RolUsuario = user?.Rol ?? "Comprador";
            return response;
        }

        public async Task<IEnumerable<ComentarioResponseDto>> GetAllAsync()
        {
            var comentarios = await _repository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<ComentarioResponseDto>>(comentarios);
            foreach (var dto in dtos)
            {
                var user = await _usuarioRepository.GetByIdAsync(dto.UsuarioId);
                dto.NombreUsuario = user?.NombreCompleto ?? "Usuario Desconocido";
                dto.RolUsuario = user?.Rol ?? "Comprador";
            }
            return dtos.Where(c => !c.Eliminado).OrderByDescending(c => c.FechaEnvio);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var comentario = await _repository.GetByIdAsync(id);
            if (comentario == null) return false;
            comentario.Eliminado = true;
            await _repository.UpdateAsync(comentario);
            return true;
        }
    }
}
