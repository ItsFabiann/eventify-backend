using Microsoft.EntityFrameworkCore;
using EventifyAPI.Domain.Models;

namespace EventifyAPI.Infrastructure.Data
{
    public class EventifyDbContext : DbContext
    {
        public EventifyDbContext(DbContextOptions<EventifyDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Evento> Eventos { get; set; } = null!;
        public DbSet<Boleto> Boletos { get; set; } = null!;
        public DbSet<Pago> Pagos { get; set; } = null!;
        public DbSet<Comentario> Comentarios { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(e => e.UsuarioId);
                entity.Property(e => e.UsuarioId).ValueGeneratedOnAdd();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
                entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
                entity.Property(e => e.NombreCompleto).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Rol).HasMaxLength(20).IsRequired();
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()").IsRequired();
                entity.Property(e => e.Activo).IsRequired().HasDefaultValue(true);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<Evento>(entity =>
            {
                entity.ToTable("Eventos");
                entity.HasKey(e => e.EventoId);
                entity.Property(e => e.EventoId).ValueGeneratedOnAdd();
                entity.Property(e => e.Titulo).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.Property(e => e.FechaEvento).IsRequired();
                entity.Property(e => e.Ubicacion).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Capacidad).IsRequired().HasDefaultValue(1000);
                entity.Property(e => e.AsientosDisponibles).IsRequired().HasDefaultValue(1000);
                entity.Property(e => e.Categoria).HasMaxLength(50);
                entity.Property(e => e.OrganizadorId).IsRequired();
                entity.Property(e => e.Precio).HasColumnType("decimal(10,2)").IsRequired().HasDefaultValue(0.00m);
                entity.Property(e => e.Imagen).HasMaxLength(500);
                entity.Property(e => e.EstadoEvento).HasMaxLength(20).IsRequired().HasDefaultValue("Activo");
                entity.Property(e => e.FechaModificacion);
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()").IsRequired();

                entity.HasOne(e => e.Organizador)
                      .WithMany(u => u.EventosOrganizados)
                      .HasForeignKey(e => e.OrganizadorId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.FechaEvento);
                entity.HasIndex(e => e.OrganizadorId);
            });

            modelBuilder.Entity<Boleto>(entity =>
            {
                entity.ToTable("Boletos");
                entity.HasKey(e => e.BoletoId);
                entity.Property(e => e.BoletoId).ValueGeneratedOnAdd();
                entity.Property(e => e.EventoId).IsRequired();
                entity.Property(e => e.UsuarioId).IsRequired();
                entity.Property(e => e.TipoBoleto).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Cantidad).IsRequired().HasDefaultValue(1);
                entity.Property(e => e.Precio).HasColumnType("decimal(10,2)").IsRequired();
                entity.Property(e => e.CodigoQR).HasMaxLength(255);
                entity.Property(e => e.Estado).HasMaxLength(20).IsRequired();
                entity.Property(e => e.FechaCompra).HasDefaultValueSql("GETDATE()").IsRequired();

                entity.HasOne(b => b.Evento)
                      .WithMany(e => e.Boletos)
                      .HasForeignKey(b => b.EventoId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(b => b.Usuario)
                      .WithMany(u => u.BoletosComprados)
                      .HasForeignKey(b => b.UsuarioId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => e.EventoId);
                entity.HasIndex(e => e.UsuarioId);
            });

            modelBuilder.Entity<Pago>(entity =>
            {
                entity.ToTable("Pagos");
                entity.HasKey(e => e.PagoId);
                entity.Property(e => e.PagoId).ValueGeneratedOnAdd();
                entity.Property(e => e.BoletoId).IsRequired();
                entity.Property(e => e.Monto).HasColumnType("decimal(10,2)").IsRequired();
                entity.Property(e => e.MetodoPago).HasMaxLength(50).IsRequired();
                entity.Property(e => e.IdTransaccion).HasMaxLength(100);
                entity.Property(e => e.Estado).HasMaxLength(20).IsRequired();
                entity.Property(e => e.FechaPago).HasDefaultValueSql("GETDATE()").IsRequired();

                entity.HasOne<Boleto>()
                      .WithMany()
                      .HasForeignKey("BoletoId")
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.BoletoId);
            });

            modelBuilder.Entity<Comentario>(entity =>
            {
                entity.ToTable("Comentarios");
                entity.HasKey(e => e.ComentarioId);
                entity.Property(e => e.ComentarioId).ValueGeneratedOnAdd();
                entity.Property(e => e.UsuarioId).IsRequired();
                entity.Property(e => e.Mensaje).IsRequired();
                entity.Property(e => e.FechaEnvio).HasDefaultValueSql("GETDATE()").IsRequired();
                entity.Property(e => e.Eliminado).IsRequired().HasDefaultValue(false);

                entity.HasOne(c => c.Usuario)
                      .WithMany(u => u.Comentarios)
                      .HasForeignKey(c => c.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UsuarioId);
                entity.HasIndex(e => e.FechaEnvio);
            });
        }
    }
}
