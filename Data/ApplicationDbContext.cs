using Microsoft.EntityFrameworkCore;
using jbp_wapp.Models;
using Microsoft.Extensions.Logging;

namespace jbp_wapp.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ILogger<ApplicationDbContext> _logger;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ILogger<ApplicationDbContext> logger) : base(options) 
        { 
            _logger = logger;

            try 
            {
                if (Database.CanConnect())
                {
                    _logger.LogInformation("Conexion exitosa a la base de datos");
                }
                else 
                {
                    _logger.LogInformation("No se pudo establecer la conexion a la BD");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar conectar a la base de datos");
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            optionsBuilder
                .UseLoggerFactory(loggerFactory)
                .EnableSensitiveDataLogging();

            base.OnConfiguring(optionsBuilder);
        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Experiencia> Experiencias { get; set; }
        public DbSet<Profesion> Profesiones { get; set; }
        public DbSet<PerfilPostulante> PerfilPostulante { get; set; }
        public DbSet<Vacante> Vacantes { get; set; }
        public DbSet<Aplicacion> Aplicaciones { get; set; }
        public DbSet<EstadosAplicacion> EstadosAplicaciones { get; set; }
        public DbSet<HistorialEstadoAplicaciones> HistorialEstadosAplicaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de relaciones y restricciones adicionales si son necesarias
            // Relacion Usuario - Genero 1-1
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Genero)
                .WithMany()
                .HasForeignKey(u => u.IdGenero);
            // Relacion Usuario - Rol 1-1
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany()
                .HasForeignKey(u => u.IdRol);
            // Relacion Usuario - Departamento 1-1
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Departamento)
                .WithMany()
                .HasForeignKey(u => u.IdDepartamento);
            // Relacion Usuario - Experiencia 1-1
            modelBuilder.Entity<PerfilPostulante>()
                .HasOne(u => u.Experiencia)
                .WithMany()
                .HasForeignKey(u => u.IdExperiencia);
            // Relacion Usuario - Profesion 1-1
            modelBuilder.Entity<PerfilPostulante>()
                .HasOne(u => u.Profesion)
                .WithMany()
                .HasForeignKey(u => u.IdProfesion);
            // Relacion PerfilPostulante - Usuario 1-1 (solo si rol es "Postulante")
            modelBuilder.Entity<PerfilPostulante>()
                .HasOne(p => p.Usuario)
                .WithOne()
                .HasForeignKey<PerfilPostulante>(p => p.IdUsuario)
                .HasConstraintName("FK_PerfilPostulante_Usuario_Postulante");
                //.OnDelete(DeleteBehavior.Cascade)

            // Restricción para que un PerfilPostulante solo exista si el Usuario tiene rol de Postulante
            modelBuilder.Entity<PerfilPostulante>()
                .HasOne(p => p.Usuario)
                .WithOne()
                .HasForeignKey<PerfilPostulante>(p => p.IdUsuario)
                .IsRequired();
            
            // Relación Usuario - Vacantes 1-N (solo si el Usuario tiene rol de "Reclutador")
            modelBuilder.Entity<Vacante>()
                .HasOne(v => v.Usuario)
                .WithMany()
                .HasForeignKey(v => v.IdUsuario);
        
            modelBuilder.Entity<Vacante>()
                .HasOne(v => v.Profesion)
                .WithMany()
                .HasForeignKey(v => v.IdProfesion);
        
            modelBuilder.Entity<Vacante>()
                .HasOne(v => v.Experiencia)
                .WithMany()
                .HasForeignKey(v => v.IdExperiencia);

            // Relación Aplicación - PerfilPostulante N-1
            modelBuilder.Entity<Aplicacion>()
                .HasOne(a => a.PerfilPostulante)
                .WithMany()
                .HasForeignKey(a => a.IdPostulante)
                .HasConstraintName("FK_Aplicacion_PerfilPostulante")
                .OnDelete(DeleteBehavior.Cascade);
            
            // Relación Aplicación - Vacantes N-1
            modelBuilder.Entity<Aplicacion>()
                .HasOne(a => a.Vacante)
                .WithMany(v => v.Aplicaciones)
                .HasForeignKey(a => a.IdVacante)
                .HasConstraintName("FK_Aplicacion_Vacante")
                .OnDelete(DeleteBehavior.Cascade);

            // Relación HistorialEstadoAplicaciones con Aplicaciones y EstadosAplicacion
            modelBuilder.Entity<HistorialEstadoAplicaciones>()
                .HasOne(h => h.Aplicacion)
                .WithMany()
                .HasForeignKey(h => h.IdAplicacion)
                .HasConstraintName("FK_HistorialEstadoAplicaciones_Aplicacion")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HistorialEstadoAplicaciones>()
                .HasOne(h => h.EstadosAplicacion)
                .WithMany()
                .HasForeignKey(h => h.IdEstadoAplicacion)
                .HasConstraintName("FK_HistorialEstadoAplicaciones_EstadoAplicacion")
                .OnDelete(DeleteBehavior.Cascade);
                }
    }
}
