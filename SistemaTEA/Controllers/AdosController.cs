using Microsoft.AspNetCore.Mvc;
using SistemaTEA.Models;
using SistemaTEA.ViewModels; // Add this using directive

public class ADOSController : Controller
{
    private readonly EvaluacionContext _context;
    public ADOSController(EvaluacionContext context)
    {
        _context = context;
    }

    public IActionResult IniciarEvaluacion(int evaluacionId)
    {
        var preguntas = _context.PreguntasADOS2
                                .Where(p => p.EsActiva)
                                .OrderBy(p => p.NumeroPregunta)
                                .ToList();

        var vm = new EvaluacionADOSViewModel
        {
            EvaluacionID = evaluacionId,
            Preguntas = preguntas.Select(p => new RespuestaADOSViewModel
            {
                PreguntaID = p.PreguntaID,
                NumeroPregunta = p.NumeroPregunta,
                TextoPregunta = p.TextoPregunta
            }).ToList()
        };
        return View(vm);
    }

    [HttpPost]
    public IActionResult GuardarEvaluacion(EvaluacionADOSViewModel modelo)
    {
        foreach (var r in modelo.Preguntas)
        {
            _context.RespuestasADOS2.Add(new RespuestaADOS2
            {
                EvaluacionID = modelo.EvaluacionID,
                PreguntaID = r.PreguntaID,
                Puntuacion = r.Puntuacion,
                Observaciones = r.Observaciones,
                Comentarios = r.Comentarios,
                FechaRespuesta = DateTime.Now
            });
        }
        _context.SaveChanges();
        return RedirectToAction("DetalleEvaluacion", new { id = modelo.EvaluacionID });
    }
}
