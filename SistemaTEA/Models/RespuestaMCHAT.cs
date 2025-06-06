using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SistemaTEA.Models
{
    public class RespuestaMCHAT
    {
        [Key]
        public int RespuestaID { get; set; }

        public int EvaluacionID { get; set; }

        public int PreguntaID { get; set; }

        public bool Respuesta { get; set; }

        public DateTime FechaRespuesta { get; set; } = DateTime.Now;

        [ForeignKey("EvaluacionID")]
        public virtual Evaluacion Evaluacion { get; set; }

        [ForeignKey("PreguntaID")]
        public virtual PreguntaMCHAT Pregunta { get; set; }
    }

}
