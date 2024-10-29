using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jbp_wapp.Models
{
    // [Table("Cls_Generos")]
    public class Genero
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }

        // Relación con TblUsuarios
       //  public ICollection<TblUsuario> Tbl_Usuarios { get; set; }
    }
}
