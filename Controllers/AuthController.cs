using Microsoft.AspNetCore.Mvc;
using EventifyAPI.Application.DTOs;
using EventifyAPI.Application.Interfaces;
using EventifyAPI.Domain.Models;
using System;
using System.Threading.Tasks;

namespace EventifyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUsuarioService _usuarioService;

        public AuthController(IAuthService authService, IUsuarioService usuarioService)
        {
            _authService = authService;
            _usuarioService = usuarioService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var response = await _authService.LoginAsync(request);
                var user = await _usuarioService.GetByEmailAsync(request.Email);
                if (user != null)
                {
                    response.UsuarioId = user.UsuarioId;
                    if (!user.Activo)
                        throw new UnauthorizedAccessException("Cuenta desactivada. Contacta al administrador.");
                }

                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error en login: {ex.Message}");
            }
        }

        [HttpPost("admin-login")]
        public async Task<ActionResult<LoginResponseDto>> AdminLogin([FromBody] AdminLoginRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            const string ADMIN_CLAVE = "adminKey123";

            if (request.Clave != ADMIN_CLAVE)
                return Unauthorized("Clave de admin inválida");

            var adminUser = new Usuario
            {
                UsuarioId = 1,
                Email = "admin@eventify.com",
                Rol = "Admin"
            };

            var token = _authService.GenerateJwtToken(adminUser);

            return Ok(new LoginResponseDto
            {
                Token = token,
                Expiry = DateTime.UtcNow.AddHours(24),
                Rol = "Admin",
                UsuarioId = 1
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<UsuarioResponseDto>> Register([FromBody] UsuarioCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (string.IsNullOrEmpty(request.Rol))
                    request.Rol = "Comprador";
                else if (request.Rol != "Comprador" && request.Rol != "Organizador")
                    return BadRequest("Rol inválido para registro: Solo 'Comprador' o 'Organizador' permitidos");

                if (string.IsNullOrWhiteSpace(request.Dni))
                    return BadRequest("DNI requerido para rol Comprador u Organizador");

                var created = await _usuarioService.CreateAsync(request);
                return StatusCode(201, created);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("ya está registrado"))
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("DNI"))
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al registrar: {ex.Message}");
            }
        }
    }
}
