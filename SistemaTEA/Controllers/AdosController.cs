﻿using Microsoft.AspNetCore.Mvc;
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



    // Módulo T
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

        return View(viewModel);
    }



    public IActionResult Modulo1(int evaluacionId)
    {
        var preguntas = ObtenerPreguntasPorModulo(1);
        return View("Modulo1", CrearViewModel(evaluacionId, preguntas));
    }

    // Módulo 1
    public IActionResult Modulo2(int evaluacionId)
{
    var preguntas = ObtenerPreguntasPorModulo(2);
    return View("Modulo2", CrearViewModel(evaluacionId, preguntas));
}

// Módulo 2
public IActionResult Modulo3(int evaluacionId)
{
    var preguntas = ObtenerPreguntasPorModulo(3);
    return View("Modulo3", CrearViewModel(evaluacionId, preguntas));
}

// Módulo 3
public IActionResult Modulo4(int evaluacionId)
{
    var preguntas = ObtenerPreguntasPorModulo(4);
    return View("Modulo4", CrearViewModel(evaluacionId, preguntas));
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



}
