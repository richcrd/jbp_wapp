using System.ComponentModel.DataAnnotations;

namespace jbp_wapp.Models
{
    public class ClsEstadoAplicacion
    {
        [Key]
        public int Id_Estado_Aplicacion { get; set; }
        public string Estado_Aplicacion { get; set; }

        // Relación con TblHistorialEstadoAplicaciones
        public ICollection<TblHistorialEstadoAplicacion> HistorialEstadoAplicaciones { get; set; }
    }
}
