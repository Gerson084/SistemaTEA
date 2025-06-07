using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SistemaTEA.Models
{
    public class Evaluacion
    {
        [Key]
        public int EvaluacionID { get; set; }

        public int PacienteID { get; set; }

        public int TipoTestID { get; set; }

        public int UsuarioEvaluadorID { get; set; }

        public DateTime FechaEvaluacion { get; set; } = DateTime.Now;

        public int? ModuloADOS2ID { get; set; }

        public int? AreaADIRID { get; set; }

        public string EstadoEvaluacion { get; set; } = "En Proceso";

        public string? ObservacionesGenerales { get; set; }


        public decimal? PuntajeTotal { get; set; }

       
    }

}
