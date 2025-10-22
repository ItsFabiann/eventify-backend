using AutoMapper;
using EventifyAPI.Application.DTOs;
using EventifyAPI.Application.Interfaces;
using EventifyAPI.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EventifyAPI.Application.Services
{
    public class EventoService : IEventoService
    {
        private readonly IEventoRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUsuarioRepository _usuarioRepository;

        public EventoService(IEventoRepository repository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<EventoResponseDto>> GetAllAsync()
        {
            return await GetAllAsync(null, null, null, null);
        }

        public async Task<IEnumerable<EventoResponseDto>> GetAllAsync(string? categoria = null, decimal? precioMin = null, decimal? precioMax = null, string? busqueda = null)
        {
            var eventos = await _repository.GetAllAsync();
            var query = eventos.AsQueryable();

            if (!string.IsNullOrEmpty(categoria))
                query = query.Where(e => e.Categoria == categoria);

            if (precioMin.HasValue)
                query = query.Where(e => e.Precio >= precioMin.Value);

            if (precioMax.HasValue)
                query = query.Where(e => e.Precio <= precioMax.Value);

            if (!string.IsNullOrEmpty(busqueda))
                query = query.Where(e => e.Titulo.Contains(busqueda) || (e.Descripcion != null && e.Descripcion.Contains(busqueda)));

            var result = query.ToList();
            return _mapper.Map<IEnumerable<EventoResponseDto>>(result);
        }

        public async Task<EventoResponseDto?> GetByIdAsync(int id)
        {
            var evento = await _repository.GetByIdAsync(id);
            if (evento == null) return null;

            var dto = _mapper.Map<EventoResponseDto>(evento);
            var organizador = await _usuarioRepository.GetByIdAsync(evento.OrganizadorId);
            dto.TelefonoOrganizador = organizador?.Telefono;

            return dto;
        }

        public async Task<IEnumerable<EventoResponseDto>> GetByOrganizadorIdAsync(int organizadorId)
        {
            var eventos = await _repository.GetByOrganizadorIdAsync(organizadorId);
            return _mapper.Map<IEnumerable<EventoResponseDto>>(eventos);
        }

        public async Task<EventoResponseDto> CreateAsync(EventoCreateRequestDto request)
        {
            var organizadorIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(organizadorIdClaim) || !int.TryParse(organizadorIdClaim, out var organizadorId))
                throw new UnauthorizedAccessException("Usuario no autorizado");

            if (request.FechaEvento <= DateTime.UtcNow)
                throw new InvalidOperationException("La fecha del evento debe ser futura");

            var evento = _mapper.Map<Evento>(request);
            evento.OrganizadorId = organizadorId;
            evento.AsientosDisponibles = request.Capacidad;
            evento.FechaCreacion = DateTime.UtcNow;
            evento.EstadoEvento = request.EstadoEvento ?? "Activo";

            if (request.ImagenFile != null)
            {
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.ImagenFile.FileName);
                var filePath = Path.Combine(uploadsPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.ImagenFile.CopyToAsync(stream);
                }
                evento.Imagen = "/images/" + fileName;
            }
            else if (!string.IsNullOrEmpty(request.ImagenUrl))
            {
                evento.Imagen = request.ImagenUrl;
            }

            var created = await _repository.CreateAsync(evento);
            return _mapper.Map<EventoResponseDto>(created);
        }

        public async Task<EventoResponseDto> UpdateAsync(int id, EventoUpdateRequestDto request)
        {
            var existing = await _repository.GetByIdAsync(id) ?? throw new InvalidOperationException("Evento no encontrado");

            var organizadorIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(organizadorIdClaim) || !int.TryParse(organizadorIdClaim, out var organizadorId))
                throw new UnauthorizedAccessException("Usuario no autorizado");
            if (existing.OrganizadorId != organizadorId)
                throw new UnauthorizedAccessException("Solo el organizador puede editar este evento");

            _mapper.Map(request, existing);

            if (request.Capacidad.HasValue)
            {
                int boletosVendidos = existing.Boletos?.Count() ?? 0;
                existing.AsientosDisponibles = request.Capacidad.Value - boletosVendidos;
                if (existing.AsientosDisponibles < 0)
                    throw new InvalidOperationException("No se puede reducir la capacidad por debajo de los boletos vendidos");
            }

            existing.FechaModificacion = DateTime.UtcNow;

            if (request.ImagenFile != null)
            {
                if (!string.IsNullOrEmpty(existing.Imagen) && existing.Imagen.StartsWith("/images/"))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existing.Imagen.TrimStart('/'));
                    if (File.Exists(oldFilePath))
                        File.Delete(oldFilePath);
                }

                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.ImagenFile.FileName);
                var filePath = Path.Combine(uploadsPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.ImagenFile.CopyToAsync(stream);
                }
                existing.Imagen = "/images/" + fileName;
            }
            else if (!string.IsNullOrEmpty(request.ImagenUrl))
            {
                existing.Imagen = request.ImagenUrl;
            }

            var updated = await _repository.UpdateAsync(existing);
            return _mapper.Map<EventoResponseDto>(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var evento = await _repository.GetByIdAsync(id);
            if (evento != null && evento.Boletos.Any())
                throw new InvalidOperationException("No se puede eliminar un evento con boletos asignados");
            return await _repository.DeleteAsync(id);
        }

        public async Task DesactivarEventosPorOrganizadorAsync(int organizadorId)
        {
            var eventos = await _repository.GetByOrganizadorIdAsync(organizadorId);
            foreach (var evento in eventos)
            {
                evento.EstadoEvento = "Inactivo";
                await _repository.UpdateAsync(evento);
            }
        }
    }
}
