using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jbp_wapp.Models
{
    public class Vacante
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
        public string Titulo { get; set; }
        [Required]
        [MaxLength(255)]
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaCierre { get; set; }


        // Relaciones
        public virtual Usuario? Usuario { get; set; }
        public virtual Profesion? Profesion { get; set; }
        public virtual Experiencia? Experiencia { get; set; }
    }
}
