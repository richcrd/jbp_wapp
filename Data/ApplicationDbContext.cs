using Microsoft.EntityFrameworkCore;
using jbp_wapp.Models;

namespace jbp_wapp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Genero> Generos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de relaciones y restricciones adicionales si son necesarias
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Genero)
                .WithMany()
                .HasForeignKey(u => u.IdGenero);

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany()
                .HasForeignKey(u => u.IdRol);

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Departamento)
                .WithMany()
                .HasForeignKey(u => u.IdDepartamento);
        }
    }
}
