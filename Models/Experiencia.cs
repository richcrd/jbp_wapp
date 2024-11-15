using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jbp_wapp.Models
{
    public class Experiencia
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Descripcion { get; set; }
    }
}
