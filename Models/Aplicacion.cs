using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jbp_wapp.Models
{
    public class Aplicacion
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("IdPostulante")]
        public int IdPostulante { get; set; }
        [ForeignKey("IdVacante")]
        public int IdVacante { get; set; }
        public DateTime FechaAplicacion { get; set; }


        // Relaciones
        public ICollection<HistorialEstadoAplicaciones> HistorialEstadoAplicacion { get; set; }
        public virtual PerfilPostulante? PerfilPostulante { get; set; }
        public virtual Vacante? Vacante { get; set; }
    }
}