using System.ComponentModel.DataAnnotations;

namespace jbp_wapp.Models
{
    public class TblAplicacion
    {
        [Key]
        public int Id_Aplicacion { get; set; }
        public int Id_Postulante { get; set; }
        public int Id_Vacante { get; set; }
        public DateTime Fecha_Aplicacion { get; set; }

        public TblPostulante Postulante { get; set; }
        public TblVacante Vacante { get; set; }
    }
}