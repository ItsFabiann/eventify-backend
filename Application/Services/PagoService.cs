using AutoMapper;
using EventifyAPI.Application.DTOs;
using EventifyAPI.Application.Interfaces;
using EventifyAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventifyAPI.Application.Services
{
    public class PagoService : IPagoService
    {
        private readonly IPagoRepository _repository;
        private readonly IBoletoRepository _boletoRepository;
        private readonly IEventoRepository _eventoRepository;
        private readonly IMapper _mapper;

        public PagoService(IPagoRepository repository, IBoletoRepository boletoRepository, IEventoRepository eventoRepository, IMapper mapper)
        {
            _repository = repository;
            _boletoRepository = boletoRepository;
            _eventoRepository = eventoRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PagoResponseDto>> GetAllAsync()
        {
            var pagos = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<PagoResponseDto>>(pagos);
        }

        public async Task<PagoResponseDto?> GetByIdAsync(int id)
        {
            var pago = await _repository.GetByIdAsync(id);
            return pago == null ? null : _mapper.Map<PagoResponseDto>(pago);
        }

        public async Task<PagoResponseDto> CreateAsync(PagoCreateRequestDto request)
        {
            var boleto = await _boletoRepository.GetByIdAsync(request.BoletoId) ?? throw new InvalidOperationException("Boleto no encontrado");
            if (boleto.Estado != "Disponible") throw new InvalidOperationException("Boleto no está listo para pago");

            var pago = _mapper.Map<Pago>(request);
            pago.FechaPago = DateTime.UtcNow;
            pago.Estado = "Pendiente";
            pago.Comision = request.Comision;

            var created = await _repository.CreateAsync(pago);
            return _mapper.Map<PagoResponseDto>(created);
        }

        public async Task<PagoResponseDto> UpdateAsync(int id, PagoUpdateRequestDto request)
        {
            var existing = await _repository.GetByIdAsync(id) ?? throw new InvalidOperationException("Pago no encontrado");
            if (request.Estado != null) existing.Estado = request.Estado;
            if (request.IdTransaccion != null) existing.IdTransaccion = request.IdTransaccion;
            if (request.Comision.HasValue) existing.Comision = request.Comision.Value;
            existing.FechaModificacion = DateTime.UtcNow;

            if (request.Estado == "Exitoso")
            {
                var boleto = await _boletoRepository.GetByIdAsync(existing.BoletoId);
                if (boleto != null)
                {
                    boleto.Estado = "Comprado";
                    boleto.FechaModificacion = DateTime.UtcNow;
                    await _boletoRepository.UpdateAsync(boleto);

                    var evento = await _eventoRepository.GetByIdAsync(boleto.EventoId);
                    if (evento != null)
                    {
                        evento.AsientosDisponibles -= boleto.Cantidad;
                        await _eventoRepository.UpdateAsync(evento);
                    }
                }
            }

            var updated = await _repository.UpdateAsync(existing);
            return _mapper.Map<PagoResponseDto>(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pago = await _repository.GetByIdAsync(id);
            if (pago == null || pago.Estado != "Pendiente")
                return false;
            return await _repository.DeleteAsync(id);
        }

        public async Task<ResultadoPagoDto> ProcesarPagoSimuladoAsync(int eventoId, int usuarioId, int cantidad, string metodoPago, decimal monto, string telefono, string codigoAprobacion)
        {
            Console.WriteLine($"Procesando simulación: EventoId={eventoId}, UsuarioId={usuarioId}, Cantidad={cantidad}");
            var evento = await _eventoRepository.GetByIdAsync(eventoId);
            if (evento == null)
                return new ResultadoPagoDto { Exito = false, Mensaje = "Evento no encontrado" };
            if (evento.AsientosDisponibles < cantidad)
                return new ResultadoPagoDto { Exito = false, Mensaje = "Asientos insuficientes" };

            var boleto = new Boleto
            {
                EventoId = eventoId,
                UsuarioId = usuarioId,
                TipoBoleto = "General",
                NumeroBoleto = $"EV{eventoId}-G{_boletoRepository.GetNextId()}",
                Cantidad = cantidad,
                Precio = monto,
                CodigoQR = $"QR-{Guid.NewGuid()}",
                Estado = "Comprado",
                FechaCompra = DateTime.UtcNow
            };

            try
            {
                await _boletoRepository.AddAsync(boleto);
                evento.AsientosDisponibles -= cantidad;
                await _eventoRepository.UpdateAsync(evento);
                Console.WriteLine($"Simulación exitosa: Boleto guardado, asientos actualizados");
                return new ResultadoPagoDto { Exito = true, Mensaje = "Pago simulado exitoso" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new ResultadoPagoDto { Exito = false, Mensaje = $"Error: {ex.Message}" };
            }
        }
    }
}
