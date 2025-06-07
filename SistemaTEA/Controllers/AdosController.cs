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




    private EvaluacionADOSViewModel CrearViewModel(int evaluacionId, List<PreguntaADOS2> preguntas)
    {
        return new EvaluacionADOSViewModel
        {
            EvaluacionID = evaluacionId,
            Preguntas = preguntas.Select(p => new RespuestaADOSViewModel
            {
                PreguntaID = p.PreguntaID,
                NumeroPregunta = p.NumeroPregunta,
                TextoPregunta = p.TextoPregunta
            }).ToList()
        };
    }

    private List<PreguntaADOS2> ObtenerPreguntasPorModulo(int moduloId)
    {
        return _context.PreguntasADOS2
                       .Where(p => p.EsActiva && p.ModuloID == moduloId)
                       .OrderBy(p => p.NumeroPregunta)
                       .ToList();
    }

   
    public IActionResult ModuloT(int evaluacionId)
    {


        int moduloId = 5; // O el módulo que corresponda para "T".

        // Traer las preguntas del módulo 5
        var preguntas = _context.PreguntasADOS2
            .Where(p => p.ModuloID == moduloId)
            .OrderBy(p => p.NumeroPregunta)
            .ToList();

        var viewModel = new ADOSEvaluacionViewModel
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

    public IActionResult Modulo1(int evaluacionId)
    {
         int moduloId = 1; // O el módulo que corresponda para "T".

        // Traer las preguntas del módulo 5
        var preguntas = _context.PreguntasADOS2
            .Where(p => p.ModuloID == moduloId)
            .OrderBy(p => p.NumeroPregunta)
            .ToList();

        var viewModel = new ADOSEvaluacionViewModel
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





  
    // Módulo 1
    public IActionResult Modulo2(int evaluacionId)
    {

        int moduloId = 2; // O el módulo que corresponda para "T".

        // Traer las preguntas del módulo 5
        var preguntas = _context.PreguntasADOS2
            .Where(p => p.ModuloID == moduloId)
            .OrderBy(p => p.NumeroPregunta)
            .ToList();

        var viewModel = new ADOSEvaluacionViewModel
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
    public IActionResult Modulo3(int evaluacionId)
    {

        int moduloId = 3; // O el módulo que corresponda para "T".

        // Traer las preguntas del módulo 5
        var preguntas = _context.PreguntasADOS2
            .Where(p => p.ModuloID == moduloId)
            .OrderBy(p => p.NumeroPregunta)
            .ToList();

        var viewModel = new ADOSEvaluacionViewModel
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
    public IActionResult Modulo4(int evaluacionId)
    {

        int moduloId = 4; // O el módulo que corresponda para "T".

        // Traer las preguntas del módulo 5
        var preguntas = _context.PreguntasADOS2
            .Where(p => p.ModuloID == moduloId)
            .OrderBy(p => p.NumeroPregunta)
            .ToList();

        var viewModel = new ADOSEvaluacionViewModel
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

    // Módulo 4



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

   /* [HttpPost]
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
    }*/







    public IActionResult Evaluacion(int evaluacionId, int moduloId = 5)
    {
        var model = new ADOSEvaluacionViewModel
        {
            EvaluacionID = evaluacionId,
            ModuloID = moduloId,
            Preguntas = GetPreguntasByModulo(moduloId),
            RespuestasExistentes = GetRespuestasExistentes(evaluacionId)
        };

        return View(model);
    }

    private IEnumerable<PreguntaADOS2> GetPreguntasByModulo(int moduloId)
    {
        // Consulta a la base de datos
        return _context.PreguntasADOS2
            .Where(p => p.ModuloID == moduloId)
            .OrderBy(p => p.NumeroPregunta)
            .ToList();
    }

    private IEnumerable<RespuestaADOS2> GetRespuestasExistentes(int evaluacionId)
    {
        return _context.RespuestasADOS2
                       .Where(r => r.EvaluacionID == evaluacionId)
                       .ToList();
    }









    [HttpPost]
    public IActionResult GuardarRespuestaIndividual(int EvaluacionID, int PreguntaID, int? Puntuacion, string Comentarios, string Observaciones)
    {
        try
        {
            // Validar que los datos requeridos estén presentes
            if (EvaluacionID <= 0 || PreguntaID <= 0)
            {
                TempData["ErrorPregunta"] = "EvaluacionID y PreguntaID son requeridos.";
                TempData["PreguntaID"] = PreguntaID;
                return RedirectToAction("FormularioEvaluacion", new { id = EvaluacionID });
            }

            // Verificar si ya existe una respuesta para esta pregunta en esta evaluación
            var respuestaExistente = _context.RespuestasADOS2
                .FirstOrDefault(r => r.EvaluacionID == EvaluacionID && r.PreguntaID == PreguntaID);

            if (respuestaExistente != null)
            {
                // Actualizar respuesta existente
                respuestaExistente.Puntuacion = Puntuacion;
                respuestaExistente.Comentarios = Comentarios;
                respuestaExistente.Observaciones = Observaciones;
                respuestaExistente.FechaRespuesta = DateTime.Now;
            }
            else
            {
                // Crear nueva respuesta
                var nuevaRespuesta = new RespuestaADOS2
                {
                    EvaluacionID = EvaluacionID,
                    PreguntaID = PreguntaID,
                    Puntuacion = Puntuacion,
                    Comentarios = Comentarios,
                    Observaciones = Observaciones,
                    FechaRespuesta = DateTime.Now
                };
                _context.RespuestasADOS2.Add(nuevaRespuesta);
            }

            _context.SaveChanges();

            TempData["SuccessPregunta"] = "Pregunta guardada correctamente.";
            TempData["PreguntaID"] = PreguntaID;
          //  return RedirectToAction("FormularioEvaluacion", new { id = EvaluacionID });
            return RedirectToModuloCorrespondiente(EvaluacionID);
        }
        catch (Exception ex)
        {
            TempData["ErrorPregunta"] = $"Ocurrió un error: {ex.Message}";
            TempData["PreguntaID"] = PreguntaID;
           // return RedirectToAction("FormularioEvaluacion", new { id = EvaluacionID });
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
            return RedirectToAction("BuscarPaciente");
        }

        // Determinar el módulo según ModuloADOS2ID
        string moduloAction = evaluacion.ModuloADOS2ID switch
        {
            1 => "ModuloT",
            2 => "Modulo1",
            3 => "Modulo2",
            4 => "Modulo3",
            5 => "Modulo4",
            _ => null
        };

        if (moduloAction == null)
        {
            TempData["Error"] = "Módulo inválido.";
            return RedirectToAction("BuscarPaciente");
        }

        return RedirectToAction(moduloAction, new { evaluacionId = evaluacionId });
    }


    








}