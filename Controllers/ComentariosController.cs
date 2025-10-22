using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EventifyAPI.Application.DTOs;
using EventifyAPI.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace EventifyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComentariosController : ControllerBase
    {
        private readonly IComentarioService _service;

        public ComentariosController(IComentarioService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Policy = "LoggedIn")]
        public async Task<ActionResult<ComentarioResponseDto>> CrearComentario([FromBody] ComentarioCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var usuarioId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (usuarioId <= 0)
                    return Unauthorized("Usuario no válido");

                var created = await _service.CrearAsync(request.Mensaje, usuarioId);
                return StatusCode(201, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al enviar comentario: {ex.Message}");
            }
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<IEnumerable<ComentarioResponseDto>>> GetComentarios()
        {
            try
            {
                var comentarios = await _service.GetAllAsync();
                return Ok(comentarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener comentarios: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> EliminarComentario(int id)
        {
            try
            {
                var deleted = await _service.EliminarAsync(id);
                if (!deleted)
                    return NotFound("Comentario no encontrado");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar comentario: {ex.Message}");
            }
        }
    }
}
