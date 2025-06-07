using SistemaTEA.Models;

namespace SistemaTEA.ViewModels
{
    public class ADIREvaluacionViewModel
    {
        public int EvaluacionID { get; set; }
        public int ModuloID { get; set; }
        public string NombrePaciente { get; set; }
        public IEnumerable<PreguntaADIR> Preguntas { get; set; }
        public IEnumerable<RespuestaADIR> RespuestasExistentes { get; set; }

    }
}
