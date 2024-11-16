using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jbp_wapp.Models
{
    public class PerfilPostulante
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("IdUsuario")]
        public int IdUsuario { get; set; }
        [ForeignKey("IdProfesion")]
        public int IdProfesion { get; set; }
        [ForeignKey("IdExperiencia")]
        public int IdExperiencia { get; set; }
        [Required]
        [MaxLength(100)]
        public byte[] CV { get; set; }

        [NotMapped]
        public string Nombre => Usuario?.Nombre;
        public string Apellido => Usuario?.Apellido;
        public string Correo => Usuario?.Correo;
        public string Departamento => Usuario?.Departamento?.Nombre;

        // Relaciones
        public virtual Usuario Usuario { get; set; }
        public virtual Profesion? Profesion { get; set; }
        public virtual Experiencia? Experiencia { get; set; }
    }
}
