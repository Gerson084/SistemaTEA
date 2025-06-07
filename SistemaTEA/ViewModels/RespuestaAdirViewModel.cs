namespace SistemaTEA.ViewModels
{
    public class RespuestaAdirViewModel
    {
        public int PreguntaID { get; set; }
        public int NumeroPregunta { get; set; }
        public string TextoPregunta { get; set; }
        public int? Puntuacion { get; set; }
        public string Observaciones { get; set; }
        public string Comentarios { get; set; }
    }
    public class EvaluacionADIRViewModel
    {
        public int EvaluacionID { get; set; }
        public List<RespuestaAdirViewModel> Preguntas { get; set; }
    }
}
