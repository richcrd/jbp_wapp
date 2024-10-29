using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jbp_wapp.Models
{
    // [Table("Cls_Departamentos")]
    public class Departamento
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }

        // public ICollection<TblUsuario> Tbl_Usuarios { get; set; }
    }
}
