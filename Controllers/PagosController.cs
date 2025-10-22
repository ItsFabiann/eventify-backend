using EventifyAPI.Application.DTOs;
using EventifyAPI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EventifyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagosController : ControllerBase
    {
        private readonly IPagoService _service;

        public PagosController(IPagoService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Policy = "LoggedIn")]
        public async Task<ActionResult<IEnumerable<PagoResponseDto>>> GetPagos()
        {
            try
            {
                var pagos = await _service.GetAllAsync();
                return Ok(pagos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "LoggedIn")]
        public async Task<ActionResult<PagoResponseDto>> GetPago(int id)
        {
            try
            {
                var pago = await _service.GetByIdAsync(id);
                if (pago == null)
                    return NotFound($"Pago con ID {id} no encontrado");
                return Ok(pago);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Policy = "LoggedIn")]
        public async Task<ActionResult<PagoResponseDto>> CreatePago([FromBody] PagoCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAsync(request);
                return CreatedAtAction(nameof(GetPago), new { id = created.PagoId }, created);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Boleto no encontrado") || ex.Message.Contains("no está listo"))
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear pago: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "LoggedIn")]
        public async Task<ActionResult<PagoResponseDto>> UpdatePago(int id, [FromBody] PagoUpdateRequestDto request)
        {
            if (id <= 0 || !ModelState.IsValid)
                return BadRequest("ID inválido o datos incorrectos");

            try
            {
                var updated = await _service.UpdateAsync(id, request);
                return Ok(updated);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("no encontrado"))
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeletePago(int id)
        {
            if (id <= 0)
                return BadRequest("ID inválido");

            try
            {
                var deleted = await _service.DeleteAsync(id);
                if (!deleted)
                    return NotFound($"Pago con ID {id} no encontrado o no pendiente");
                return NoContent();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("pendientes"))
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar: {ex.Message}");
            }
        }

        [HttpPost("procesar-simulado")]
        [Authorize(Policy = "LoggedIn")]
        public async Task<IActionResult> ProcesarPagoSimulado([FromBody] ProcesarPagoSimuladoRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var resultado = await _service.ProcesarPagoSimuladoAsync(request.EventoId, usuarioId, request.Cantidad, request.MetodoPago, request.Monto, request.Telefono, request.CodigoAprobacion);
                if (!resultado.Exito)
                    return BadRequest(resultado.Mensaje);
                return Ok("Pago simulado exitoso");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al procesar simulación: {ex.Message}");
            }
        }
    }
}
