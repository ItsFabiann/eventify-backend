using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EventifyAPI.Application.DTOs;
using EventifyAPI.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventifyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventosController : ControllerBase
    {
        private readonly IEventoService _service;

        public EventosController(IEventoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventoResponseDto>>> GetEventos(
            [FromQuery] string? categoria = null,
            [FromQuery] decimal? precioMin = null,
            [FromQuery] decimal? precioMax = null,
            [FromQuery] string? busqueda = null)
        {
            try
            {
                if (precioMin.HasValue && precioMin < 0) return BadRequest("Precio mínimo no puede ser negativo");
                if (precioMax.HasValue && precioMax < 0) return BadRequest("Precio máximo no puede ser negativo");
                if (precioMin.HasValue && precioMax.HasValue && precioMin > precioMax) return BadRequest("Precio mínimo no puede ser mayor que máximo");

                var eventos = await _service.GetAllAsync(categoria, precioMin, precioMax, busqueda);
                return Ok(eventos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventoResponseDto>> GetEvento(int id)
        {
            try
            {
                var evento = await _service.GetByIdAsync(id);
                if (evento == null)
                    return NotFound($"Evento con ID {id} no encontrado");
                return Ok(evento);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("organizador/{id}")]
        [Authorize(Policy = "LoggedIn")]
        public async Task<ActionResult<IEnumerable<EventoResponseDto>>> GetEventosByOrganizador(int id)
        {
            try
            {
                var eventos = await _service.GetByOrganizadorIdAsync(id);
                return Ok(eventos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Policy = "OrganizadorOnly")]
        public async Task<ActionResult<EventoResponseDto>> CreateEvento([FromForm] EventoCreateRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest($"Datos inválidos: {string.Join(", ", errors)}");
            }

            try
            {
                var created = await _service.CreateAsync(request);
                return CreatedAtAction(nameof(GetEvento), new { id = created.EventoId }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "OrganizadorOnly")]
        public async Task<ActionResult<EventoResponseDto>> UpdateEvento(int id, [FromForm] EventoUpdateRequestDto request)
        {
            if (id <= 0 || !ModelState.IsValid)
                return BadRequest("ID inválido o datos incorrectos");

            if (request.ImagenFile != null && request.ImagenFile.Length > 5 * 1024 * 1024)
                return BadRequest("Imagen demasiado grande (máx 5MB)");

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
        public async Task<IActionResult> DeleteEvento(int id)
        {
            if (id <= 0)
                return BadRequest("ID inválido");

            try
            {
                var deleted = await _service.DeleteAsync(id);
                if (!deleted)
                    return NotFound($"Evento con ID {id} no encontrado");
                return NoContent();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("boletos"))
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar: {ex.Message}");
            }
        }
    }
}
