using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EventifyAPI.Application.DTOs;
using EventifyAPI.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;

namespace EventifyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _service;

        public UsuariosController(IUsuarioService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Policy = "LoggedIn")]
        public async Task<ActionResult<IEnumerable<UsuarioResponseDto>>> GetUsuarios()
        {
            try
            {
                var usuarios = await _service.GetAllAsync();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "LoggedIn")]
        public async Task<ActionResult<UsuarioResponseDto>> GetUsuario(int id)
        {
            try
            {
                var usuario = await _service.GetByIdAsync(id);
                if (usuario == null)
                    return NotFound($"Usuario con ID {id} no encontrado");
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<UsuarioResponseDto>> CreateUsuario([FromBody] UsuarioCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAsync(request);
                return CreatedAtAction(nameof(GetUsuario), new { id = created.UsuarioId }, created);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("ya está registrado") || ex.Message.Contains("DNI"))
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear usuario: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<UsuarioResponseDto>> UpdateUsuario(int id, [FromBody] UsuarioUpdateRequestDto request)
        {
            if (id <= 0 || !ModelState.IsValid)
                return BadRequest("ID inválido o datos incorrectos");

            try
            {
                var updated = await _service.UpdateAsync(id, request);
                return Ok(updated);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("no encontrado") || ex.Message.Contains("DNI"))
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }
        }

        [HttpPut("{id}/activo")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateUsuarioActivo(int id, [FromBody] bool activo)
        {
            if (id <= 0)
                return BadRequest("ID inválido");

            try
            {
                var updated = await _service.UpdateActivoAsync(id, activo);
                if (!updated)
                    return NotFound($"Usuario con ID {id} no encontrado");
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar estado: {ex.Message}");
            }
        }

        [HttpPut("perfil/{id}")]
        [Authorize(Policy = "LoggedIn")]
        public async Task<ActionResult<UsuarioResponseDto>> UpdatePerfil(int id, [FromBody] UsuarioUpdateRequestDto request)
        {
            if (id <= 0 || !ModelState.IsValid)
                return BadRequest("ID inválido o datos incorrectos");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int tokenUserId) || tokenUserId != id)
                return Forbid("No puedes editar el perfil de otro usuario");

            var filteredRequest = new UsuarioUpdateRequestDto
            {
                NombreCompleto = request.NombreCompleto,
                Telefono = request.Telefono
            };

            try
            {
                var updated = await _service.UpdateAsync(id, filteredRequest);
                return Ok(updated);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("no encontrado") || ex.Message.Contains("DNI"))
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar perfil: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            if (id <= 0)
                return BadRequest("ID inválido");

            try
            {
                var deleted = await _service.DeleteAsync(id);
                if (!deleted)
                    return NotFound($"Usuario con ID {id} no encontrado");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar: {ex.Message}");
            }
        }
    }
}
