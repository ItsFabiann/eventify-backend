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
    public class BoletoService : IBoletoService
    {
        private readonly IBoletoRepository _repository;
        private readonly IEventoRepository _eventoRepository;
        private readonly IMapper _mapper;

        public BoletoService(IBoletoRepository repository, IEventoRepository eventoRepository, IMapper mapper)
        {
            _repository = repository;
            _eventoRepository = eventoRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BoletoResponseDto>> GetAllAsync()
        {
            var boletos = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<BoletoResponseDto>>(boletos);
        }

        public async Task<BoletoResponseDto?> GetByIdAsync(int id)
        {
            var boleto = await _repository.GetByIdAsync(id);
            return boleto == null ? null : _mapper.Map<BoletoResponseDto>(boleto);
        }

        public async Task<IEnumerable<BoletoResponseDto>> CreateMultipleAsync(BoletoCreateRequestDto request)
        {
            var evento = await _eventoRepository.GetByIdAsync(request.EventoId) ?? throw new InvalidOperationException("Evento no encontrado");
            if (evento.AsientosDisponibles < request.Cantidad)
                throw new InvalidOperationException($"Asientos insuficientes. Disponibles: {evento.AsientosDisponibles}");

            var boletosCreados = new List<Boleto>();
            for (int i = 0; i < request.Cantidad; i++)
            {
                var boleto = _mapper.Map<Boleto>(request);
                boleto.Precio = evento.Precio * boleto.Cantidad;
                boleto.FechaCompra = DateTime.UtcNow;
                boleto.Estado = "Disponible";

                if (string.IsNullOrEmpty(request.NumeroBoleto))
                {
                    var guidPart = Guid.NewGuid().ToString("N")[..6].ToUpper();
                    boleto.NumeroBoleto = $"EV{request.EventoId:D3}-{request.TipoBoleto}-{guidPart}";
                }
                else
                {
                    boleto.NumeroBoleto = request.NumeroBoleto;
                }

                boleto.CodigoQR = request.CodigoQR ?? $"QR-{Guid.NewGuid().ToString("N")[..8]}-{request.TipoBoleto}";

                var created = await _repository.CreateAsync(boleto);
                boletosCreados.Add(created);
            }

            evento.AsientosDisponibles -= request.Cantidad;
            await _eventoRepository.UpdateAsync(evento);

            return _mapper.Map<IEnumerable<BoletoResponseDto>>(boletosCreados);
        }

        public async Task<BoletoResponseDto> UpdateAsync(int id, BoletoUpdateRequestDto request)
        {
            var existing = await _repository.GetByIdAsync(id) ?? throw new InvalidOperationException("Boleto no encontrado");
            if (request.Estado != null) existing.Estado = request.Estado;
            if (request.CodigoQR != null) existing.CodigoQR = request.CodigoQR;
            if (request.NumeroBoleto != null) existing.NumeroBoleto = request.NumeroBoleto;
            existing.FechaModificacion = DateTime.UtcNow;

            var updated = await _repository.UpdateAsync(existing);
            return _mapper.Map<BoletoResponseDto>(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var boleto = await _repository.GetByIdAsync(id);
            if (boleto == null) return false;
            return await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<BoletoResponseDto>> GetByUsuarioIdAsync(int usuarioId)
        {
            var boletos = await _repository.GetByUsuarioIdAsync(usuarioId);
            return _mapper.Map<IEnumerable<BoletoResponseDto>>(boletos);
        }
    }
}