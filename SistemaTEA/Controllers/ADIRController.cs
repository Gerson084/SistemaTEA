using Microsoft.AspNetCore.Mvc;
using SistemaTEA.Models;
using SistemaTEA.ViewModels; 

public class ADIRController : Controller
{
    private readonly EvaluacionContext _context;
    public ADIRController(EvaluacionContext context)
    {
        _context = context;
    }




    private EvaluacionADIRViewModel CrearViewModel(int evaluacionId, List<PreguntaADIR> preguntas)
    {
        return new EvaluacionADIRViewModel
        {
            EvaluacionID = evaluacionId,
            Preguntas = preguntas.Select(p => new RespuestaAdirViewModel
            {
                PreguntaID = p.PreguntaID,
                NumeroPregunta = p.NumeroPregunta,
                TextoPregunta = p.TextoPregunta
            }).ToList()
        };
    }

    private List<PreguntaADIR> ObtenerPreguntasPorModulo(int moduloId)
    {
        return _context.PreguntasADIR
                       .Where(p => p.EsActiva && p.AreaID == moduloId)
                       .OrderBy(p => p.NumeroPregunta)
                       .ToList();
    }


    public IActionResult Area1(int evaluacionId)
    {


        int moduloId = 1; 

        var preguntas = _context.PreguntasADIR
            .Where(p => p.AreaID == moduloId)
            .OrderBy(p => p.NumeroPregunta)
            .ToList();

        var viewModel = new ADIREvaluacionViewModel
        {
            EvaluacionID = evaluacionId,
            ModuloID = moduloId,

            Preguntas = preguntas
        };

        var respuestasGuardadas = _context.RespuestasADOS2
       .Where(r => r.EvaluacionID == evaluacionId)
       .ToList();

        ViewBag.RespuestasGuardadas = respuestasGuardadas;

        return View(viewModel);
    }

    public IActionResult Area2(int evaluacionId)
    {


        int moduloId = 2;

        var preguntas = _context.PreguntasADIR
            .Where(p => p.AreaID == moduloId)
            .OrderBy(p => p.NumeroPregunta)
            .ToList();

        var viewModel = new ADIREvaluacionViewModel
        {
            EvaluacionID = evaluacionId,
            ModuloID = moduloId,

            Preguntas = preguntas
        };

        var respuestasGuardadas = _context.RespuestasADOS2
       .Where(r => r.EvaluacionID == evaluacionId)
       .ToList();

        ViewBag.RespuestasGuardadas = respuestasGuardadas;

        return View(viewModel);
    }





    public IActionResult Area3(int evaluacionId)
    {


        int moduloId = 3; 

        // Traer las preguntas del módulo 5
        var preguntas = _context.PreguntasADIR
            .Where(p => p.AreaID == moduloId)
            .OrderBy(p => p.NumeroPregunta)
            .ToList();

        var viewModel = new ADIREvaluacionViewModel
        {
            EvaluacionID = evaluacionId,
            ModuloID = moduloId,

            Preguntas = preguntas
        };

        var respuestasGuardadas = _context.RespuestasADOS2
       .Where(r => r.EvaluacionID == evaluacionId)
       .ToList();

        // Pasar las respuestas al modelo
        ViewBag.RespuestasGuardadas = respuestasGuardadas;

        return View(viewModel);
    }

    // Módulo 2
    public IActionResult Area4(int evaluacionId)
    {


        int moduloId = 4; // O el módulo que corresponda para "T".

        // Traer las preguntas del módulo 5
        var preguntas = _context.PreguntasADIR
            .Where(p => p.AreaID == moduloId)
            .OrderBy(p => p.NumeroPregunta)
            .ToList();

        var viewModel = new ADIREvaluacionViewModel
        {
            EvaluacionID = evaluacionId,
            ModuloID = moduloId,

            Preguntas = preguntas
        };

        var respuestasGuardadas = _context.RespuestasADOS2
       .Where(r => r.EvaluacionID == evaluacionId)
       .ToList();

        // Pasar las respuestas al modelo
        ViewBag.RespuestasGuardadas = respuestasGuardadas;

        return View(viewModel);
    }

    // Módulo 3
    public IActionResult Area5(int evaluacionId)
    {


        int moduloId = 5; // O el módulo que corresponda para "T".

        // Traer las preguntas del módulo 5
        var preguntas = _context.PreguntasADIR
            .Where(p => p.AreaID == moduloId)
            .OrderBy(p => p.NumeroPregunta)
            .ToList();

        var viewModel = new ADIREvaluacionViewModel
        {
            EvaluacionID = evaluacionId,
            ModuloID = moduloId,

            Preguntas = preguntas
        };

        var respuestasGuardadas = _context.RespuestasADOS2
       .Where(r => r.EvaluacionID == evaluacionId)
       .ToList();

        // Pasar las respuestas al modelo
        ViewBag.RespuestasGuardadas = respuestasGuardadas;

        return View(viewModel);
    }

    public IActionResult Area6(int evaluacionId)
    {


        int moduloId = 6; // O el módulo que corresponda para "T".

        // Traer las preguntas del módulo 5
        var preguntas = _context.PreguntasADIR
            .Where(p => p.AreaID == moduloId)
            .OrderBy(p => p.NumeroPregunta)
            .ToList();

        var viewModel = new ADIREvaluacionViewModel
        {
            EvaluacionID = evaluacionId,
            ModuloID = moduloId,

            Preguntas = preguntas
        };

        var respuestasGuardadas = _context.RespuestasADOS2
       .Where(r => r.EvaluacionID == evaluacionId)
       .ToList();

        // Pasar las respuestas al modelo
        ViewBag.RespuestasGuardadas = respuestasGuardadas;

        return View(viewModel);
    }

    public IActionResult Area7(int evaluacionId)
    {


        int moduloId = 7; 

        var preguntas = _context.PreguntasADIR
            .Where(p => p.AreaID == moduloId)
            .OrderBy(p => p.NumeroPregunta)
            .ToList();

        var viewModel = new ADIREvaluacionViewModel
        {
            EvaluacionID = evaluacionId,
            ModuloID = moduloId,

            Preguntas = preguntas
        };

        var respuestasGuardadas = _context.RespuestasADOS2
       .Where(r => r.EvaluacionID == evaluacionId)
       .ToList();


        ViewBag.RespuestasGuardadas = respuestasGuardadas;

        return View(viewModel);
    }



    public IActionResult IniciarEvaluacionADIR(int evaluacionId)
    {
        var preguntas = _context.PreguntasADIR
                                .Where(p => p.EsActiva)
                                .OrderBy(p => p.NumeroPregunta)
                                .ToList();

        var vm = new EvaluacionADIRViewModel
        {
            EvaluacionID = evaluacionId,
            Preguntas = preguntas.Select(p => new RespuestaAdirViewModel
            {
                PreguntaID = p.PreguntaID,
                NumeroPregunta = p.NumeroPregunta,
                TextoPregunta = p.TextoPregunta
            }).ToList()
        };
        return View(vm);
    }






    public IActionResult EvaluacionADIR(int evaluacionId, int moduloId = 5)
    {
        var model = new ADIREvaluacionViewModel
        {
            EvaluacionID = evaluacionId,
            ModuloID = moduloId,
            Preguntas = GetPreguntasByModulo(moduloId),
            RespuestasExistentes = GetRespuestasExistentes(evaluacionId)
        };

        return View(model);
    }

    private IEnumerable<PreguntaADIR> GetPreguntasByModulo(int moduloId)
    {
        return _context.PreguntasADIR
            .Where(p => p.AreaID == moduloId)
            .OrderBy(p => p.NumeroPregunta)
            .ToList();
    }

    private IEnumerable<RespuestaADIR> GetRespuestasExistentes(int evaluacionId)
    {
        return _context.RespuestasADIR
                       .Where(r => r.EvaluacionID == evaluacionId)
                       .ToList();
    }









    [HttpPost]
    public IActionResult GuardarRespuestaIndividual(int EvaluacionID, int PreguntaID, int? Puntuacion, string Comentarios, string Observaciones)
    {
        try
        {
            if (EvaluacionID <= 0 || PreguntaID <= 0)
            {
                TempData["ErrorPregunta"] = "EvaluacionID y PreguntaID son requeridos.";
                TempData["PreguntaID"] = PreguntaID;
                return RedirectToAction("FormularioEvaluacion", new { id = EvaluacionID });
            }

            var respuestaExistente = _context.RespuestasADIR
                .FirstOrDefault(r => r.EvaluacionID == EvaluacionID && r.PreguntaID == PreguntaID);

            if (respuestaExistente != null)
            {
                respuestaExistente.Puntuacion = Puntuacion;
                respuestaExistente.Comentarios = Comentarios;
                respuestaExistente.Observaciones = Observaciones;
                respuestaExistente.FechaRespuesta = DateTime.Now;
            }
            else
            {
                var nuevaRespuesta = new RespuestaADIR
                {
                    EvaluacionID = EvaluacionID,
                    PreguntaID = PreguntaID,
                    Puntuacion = Puntuacion,
                    Comentarios = Comentarios,
                    Observaciones = Observaciones,
                    FechaRespuesta = DateTime.Now
                };
                _context.RespuestasADIR.Add(nuevaRespuesta);
            }

            _context.SaveChanges();

            TempData["SuccessPregunta"] = "Pregunta guardada correctamente.";
            TempData["PreguntaID"] = PreguntaID;
            return RedirectToModuloCorrespondiente(EvaluacionID);
        }
        catch (Exception ex)
        {
            TempData["ErrorPregunta"] = $"Ocurrió un error: {ex.Message}";
            TempData["PreguntaID"] = PreguntaID;
            return RedirectToModuloCorrespondiente(EvaluacionID);
        }
    }


    private IActionResult RedirectToModuloCorrespondiente(int evaluacionId)
    {
        var evaluacion = _context.Evaluaciones
            .FirstOrDefault(e => e.EvaluacionID == evaluacionId);

        if (evaluacion == null)
        {
            TempData["Error"] = "No se encontró la evaluación.";
            return RedirectToAction("BuscarPacienteADIR");
        }

        string moduloAction = evaluacion.AreaADIRID switch
        {
            1 => "Area1",
            2 => "Area2",
            3 => "Area3",
            4 => "Area4",
            5 => "Area5",
            6 => "Area6",
            7 => "Area7",
           
            _ => null
        };

        if (moduloAction == null)
        {
            TempData["Error"] = "Módulo inválido.";
            return RedirectToAction("BuscarPacienteADIR");
        }

        return RedirectToAction(moduloAction, new { evaluacionId = evaluacionId });
    }











}