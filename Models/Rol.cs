using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jbp_wapp.Models
{
    public class Rol
    {
        [Key]
        public int Id { get; set; }
        // [Column("Nombre")]
        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }

        // Relación con la tabla Usuarios
        // public ICollection<TblUsuario> Tbl_Usuarios { get; set; }
    }
}
