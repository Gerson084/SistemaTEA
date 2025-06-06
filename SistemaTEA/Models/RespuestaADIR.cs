using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SistemaTEA.Models
{
    public class RespuestaADIR
    {
        [Key]
        public int RespuestaID { get; set; }

        public int EvaluacionID { get; set; }

        public int PreguntaID { get; set; }

        public int? Puntuacion { get; set; }

        public string Observaciones { get; set; }

        public string Comentarios { get; set; }

        public DateTime FechaRespuesta { get; set; } = DateTime.Now;

        [ForeignKey("EvaluacionID")]
        public virtual Evaluacion Evaluacion { get; set; }

        [ForeignKey("PreguntaID")]
        public virtual PreguntaADIR Pregunta { get; set; }
    }

}
