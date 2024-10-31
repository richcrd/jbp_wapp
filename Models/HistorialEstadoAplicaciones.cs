using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jbp_wapp.Models
{
    public class HistorialEstadoAplicaciones
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("IdAplicacion")]
        public int IdAplicacion { get; set; }
        [ForeignKey("IdEstadoAplicacion")]
        public int IdEstadoAplicacion { get; set; }
        public DateTime FechaModificacion { get; set; }


        // Relaciones
        public virtual Aplicacion? Aplicacion { get; set; }
        public virtual EstadosAplicacion? EstadosAplicacion { get; set; }
    }
}
