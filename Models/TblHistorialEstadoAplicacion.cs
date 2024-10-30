using System.ComponentModel.DataAnnotations;

namespace jbp_wapp.Models
{
    public class TblHistorialEstadoAplicacion
    {
        [Key]
        public int Id_Historial_Estado { get; set; }
        public int Id_Aplicacion { get; set; }
        public int Id_Estado_Aplicacion { get; set; }
        public DateTime Fecha_Modificacion { get; set; }

        public TblAplicacion Aplicacion { get; set; }
        public ClsEstadoAplicacion EstadoAplicacion { get; set; }
    }
}
