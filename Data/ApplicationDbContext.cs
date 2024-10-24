using Microsoft.EntityFrameworkCore;
using jbp_wapp.Models;

namespace jbp_wapp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
