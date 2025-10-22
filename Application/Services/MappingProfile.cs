using AutoMapper;
using EventifyAPI.Application.DTOs;
using EventifyAPI.Domain.Models;

namespace EventifyAPI.Application.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Usuario, UsuarioResponseDto>();
            CreateMap<UsuarioCreateRequestDto, Usuario>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true));
            CreateMap<UsuarioUpdateRequestDto, Usuario>()
                .ForMember(dest => dest.NombreCompleto, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.NombreCompleto)))
                .ForMember(dest => dest.Telefono, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Telefono)));

            CreateMap<Evento, EventoResponseDto>()
                .ForMember(dest => dest.OrganizadorNombre, opt => opt.MapFrom(src => src.Organizador.NombreCompleto))
                .ForMember(dest => dest.Imagen, opt => opt.MapFrom(src => src.Imagen))
                .ForMember(dest => dest.EstadoEvento, opt => opt.MapFrom(src => src.EstadoEvento))
                .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => src.FechaModificacion));
            CreateMap<EventoCreateRequestDto, Evento>()
                .ForMember(dest => dest.AsientosDisponibles, opt => opt.MapFrom(src => src.Capacidad))
                .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.EstadoEvento, opt => opt.MapFrom(src => src.EstadoEvento ?? "Activo"))
                .ForMember(dest => dest.Imagen, opt => opt.Ignore());
            CreateMap<EventoUpdateRequestDto, Evento>()
                .ForMember(dest => dest.Titulo, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Titulo)))
                .ForMember(dest => dest.Descripcion, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Descripcion)))
                .ForMember(dest => dest.FechaEvento, opt => opt.Condition(src => src.FechaEvento.HasValue))
                .ForMember(dest => dest.Ubicacion, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Ubicacion)))
                .ForMember(dest => dest.Capacidad, opt => opt.Condition(src => src.Capacidad.HasValue))
                .ForMember(dest => dest.Categoria, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Categoria)))
                .ForMember(dest => dest.Precio, opt => opt.Condition(src => src.Precio.HasValue))
                .ForMember(dest => dest.EstadoEvento, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.EstadoEvento)))
                .ForMember(dest => dest.Imagen, opt => opt.Ignore());

            CreateMap<Boleto, BoletoResponseDto>()
                .ForMember(dest => dest.EventoTitulo, opt => opt.MapFrom(src => src.Evento.Titulo))
                .ForMember(dest => dest.UsuarioEmail, opt => opt.MapFrom(src => src.Usuario.Email))
                .ForMember(dest => dest.NumeroBoleto, opt => opt.MapFrom(src => src.NumeroBoleto))
                .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => src.FechaModificacion));
            CreateMap<BoletoCreateRequestDto, Boleto>()
                .ForMember(dest => dest.FechaCompra, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => "Disponible"))
                .ForMember(dest => dest.NumeroBoleto, opt => opt.Ignore());
            CreateMap<BoletoUpdateRequestDto, Boleto>();

            CreateMap<Pago, PagoResponseDto>()
                .ForMember(dest => dest.BoletoNumero, opt => opt.MapFrom(src => src.Boleto.NumeroBoleto ?? src.Boleto.TipoBoleto))
                .ForMember(dest => dest.Comision, opt => opt.MapFrom(src => src.Comision))
                .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => src.FechaModificacion));
            CreateMap<PagoCreateRequestDto, Pago>()
                .ForMember(dest => dest.FechaPago, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => "Pendiente"))
                .ForMember(dest => dest.Comision, opt => opt.MapFrom(src => src.Comision));
            CreateMap<PagoUpdateRequestDto, Pago>();

            CreateMap<Comentario, ComentarioResponseDto>()
                .ForMember(dest => dest.NombreUsuario, opt => opt.MapFrom(src => src.Usuario.NombreCompleto))
                .ForMember(dest => dest.RolUsuario, opt => opt.MapFrom(src => src.Usuario.Rol));
            CreateMap<ComentarioCreateRequestDto, Comentario>()
                .ForMember(dest => dest.FechaEnvio, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Eliminado, opt => opt.MapFrom(src => false));
        }
    }
}
