using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EventifyAPI.Application.DTOs;
using EventifyAPI.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace EventifyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BoletosController : ControllerBase
    {
        private readonly IBoletoService _service;

        public BoletosController(IBoletoService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Policy = "LoggedIn")]
        public async Task<ActionResult<IEnumerable<BoletoResponseDto>>> GetBoletos()
        {
            try
            {
                var boletos = await _service.GetAllAsync();
                return Ok(boletos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "LoggedIn")]
        public async Task<ActionResult<BoletoResponseDto>> GetBoleto(int id)
        {
            try
            {
                var boleto = await _service.GetByIdAsync(id);
                if (boleto == null)
                    return NotFound($"Boleto con ID {id} no encontrado");
                return Ok(boleto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Policy = "LoggedIn")]
        public async Task<ActionResult<IEnumerable<BoletoResponseDto>>> CreateBoletos([FromBody] BoletoCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateMultipleAsync(request);
                return CreatedAtAction(nameof(GetBoleto), new { id = created.First().BoletoId }, created);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Asientos insuficientes") || ex.Message.Contains("no encontrado"))
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear boletos: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "LoggedIn")]
        public async Task<ActionResult<BoletoResponseDto>> UpdateBoleto(int id, [FromBody] BoletoUpdateRequestDto request)
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
        public async Task<IActionResult> DeleteBoleto(int id)
        {
            if (id <= 0)
                return BadRequest("ID inválido");

            try
            {
                var deleted = await _service.DeleteAsync(id);
                if (!deleted)
                    return NotFound($"Boleto con ID {id} no encontrado");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar: {ex.Message}");
            }
        }

        [HttpGet("mis-boletos")]
        [Authorize(Policy = "LoggedIn")]
        public async Task<ActionResult<IEnumerable<BoletoResponseDto>>> GetMisBoletos()
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var boletos = await _service.GetByUsuarioIdAsync(usuarioId);
                return Ok(boletos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}
