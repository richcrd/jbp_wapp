using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jbp_wapp.Models
{
    // [Table("Tbl_Usuarios")]
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("IdGenero")]
        public int IdGenero { get; set; }
        [ForeignKey("IdRol")]
        public int IdRol { get; set; }
        [ForeignKey("IdDepartamento")]
        public int IdDepartamento { get; set; }
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }
        [Required]
        [MaxLength(100)]
        public string Apellido { get; set; }
        [Required]
        [MaxLength(100)]
        public string NombreUsuario { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Correo { get; set; }
        [Required]
        [MaxLength(100)]
        public string Contrasena { get; set; }

        // Relaciones
        public virtual Genero? Genero { get; set; }
        public virtual Rol? Rol { get; set; }
        public virtual Departamento? Departamento { get; set; }
    }
}
