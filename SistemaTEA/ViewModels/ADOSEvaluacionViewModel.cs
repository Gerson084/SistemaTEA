using SistemaTEA.Models;

namespace SistemaTEA.ViewModels
{
    public class ADOSEvaluacionViewModel
    {
        
            public int EvaluacionID { get; set; }
            public int ModuloID { get; set; }
            public string NombrePaciente { get; set; }
            public IEnumerable<PreguntaADOS2> Preguntas { get; set; }
            public IEnumerable<RespuestaADOS2> RespuestasExistentes { get; set; }

    }


}
