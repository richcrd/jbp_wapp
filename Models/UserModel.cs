using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jbp_wapp.Models
{
    [Table("Tbl_Usuario")]
    public class Usuario
    {
        [Column("Id_Usuario")]
        [Key]
        public int UsuarioID { get; set; }

        [Column("Nombre_Usuario")]
        [Required]
        [StringLength(100)]
        public string NombreUsuario { get; set; }

        [Column("Correo_Usuario")]
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string CorreoUsuario { get; set; }

        [Column("Contrasena_Usuario")]
        [Required]
        [StringLength(100)]
        public string ContrasenaUsuario { get; set; }

        [Column("Id_Rol")]
        [Required]
        public int RolID { get; set; }
    }
}
