﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using SistemaTEA.Models;

namespace SistemaTEA.Controllers
{
    public class PreguntasController : Controller
    {
        private readonly EvaluacionContext _context;
        public PreguntasController(EvaluacionContext context)
        {
            _context = context;
        }
        // GET: PreguntasController
        public ActionResult Index()
        {
            var tipoTest = _context.TiposTest.ToList();

            ViewBag.PreguntasMCHAT = _context.PreguntasMCHAT.Count();
            ViewBag.PreguntasADIR = _context.PreguntasADIR.Count();
            ViewBag.AreasADIR = _context.AreasADIR.Where(a => a.EsActivo).Count();
            ViewBag.PreguntasADOS2 = _context.PreguntasADOS2.Count();
            ViewBag.ModulosADOS2 = _context.ModulosADOS2.Where(m => m.EsActivo).Count();

            ViewBag.Preguntas = _context.PreguntasMCHAT.Count();
            return View(tipoTest);
        }

        // GET: PreguntasController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PreguntasController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PreguntasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult VerPreguntas_MCHAT()
        {
            var preguntas = _context.PreguntasMCHAT.ToList();
            return View(preguntas);
        }

        //Crear controlador de MCHAT
        public ActionResult CreateMCHAT()
        {
            return View();
        }

        // POST: PreguntasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMCHAT([Bind("PreguntaID,NumeroPregunta,TextoPregunta,EsActiva")] PreguntaMCHAT pregunta_M, int id_test)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    
                    var existeNumeroPregunta = _context.PreguntasMCHAT.Any(p => p.NumeroPregunta == pregunta_M.NumeroPregunta);
                    
                    var existeTextoPregunta = _context.PreguntasMCHAT.Any(p => p.TextoPregunta == pregunta_M.TextoPregunta);

                    if (existeNumeroPregunta)
                    {
                        TempData["NumeroPregunta"] = "El número de pregunta ya existe. Por favor, elija un número diferente.";
                    }
                    else if (existeTextoPregunta)
                    {
                        TempData["TextoPregunta"] = "La pregunta ya existe. Por favor, agregue otra pregunta.";
                    }
                    else
                    {
                        _context.PreguntasMCHAT.Add(pregunta_M);
                        _context.SaveChanges();
                        return RedirectToAction("CreateMCHAT", "Preguntas", new { id = id_test });
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        public ActionResult EditMCHAT()
        {
            var preguntas = _context.PreguntasMCHAT.ToList();
            return View(preguntas);
        }

        // POST: PreguntasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMCHAT(PreguntaMCHAT model)
        {
            try
            {

                
                var existeNumeroPregunta = _context.PreguntasMCHAT.Any(p => p.NumeroPregunta == model.NumeroPregunta);
               
                var existeTextoPregunta = _context.PreguntasMCHAT.Any(p => p.TextoPregunta == model.TextoPregunta);

                if (existeNumeroPregunta)
                {
                    TempData["NumeroPregunta"] = "El número de pregunta ya existe. Por favor, elija un número diferente.";
                }
                else if (existeTextoPregunta)
                {
                    TempData["TextoPregunta"] = "La pregunta ya existe. Por favor, agregue otra pregunta.";
                }
                else
                {
                    var preguntaExistente = _context.PreguntasMCHAT.FirstOrDefault(p => p.PreguntaID == model.PreguntaID);

                    if (preguntaExistente == null)
                    {
                        return Json(new { success = false, message = "Pregunta no encontrada." });
                    }

                    preguntaExistente.TextoPregunta = model.TextoPregunta;
                    _context.SaveChanges();
                }

                

                return RedirectToAction("EditMCHAT", "Preguntas");
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error al guardar la pregunta." });
            }
        }

        [HttpGet]
        public ActionResult IniciarEvaluacionMCHART()
        {
            try
            {
                
                int id_usuario = HttpContext.Session.GetInt32("UsuarioID") ?? 0;

                if (id_usuario == 0)
                {
                    return RedirectToAction("Login", "Account"); 
                }

                var paciente = _context.Pacientes.FirstOrDefault(p => p.PadreID == id_usuario);

                if (paciente == null)
                {
                    TempData["Error"] = "No se encontró un paciente asociado a su cuenta.";
                    return RedirectToAction("Index", "Home"); 
                }

              
                var nuevaEvaluacion = new Evaluacion
                {
                    PacienteID = paciente.PacienteID,
                    TipoTestID = 1, 
                    UsuarioEvaluadorID = 11, 
                    FechaEvaluacion = DateTime.Now,
                    EstadoEvaluacion = "En Progreso" 
                };

                _context.Evaluaciones.Add(nuevaEvaluacion);
                _context.SaveChanges();

                
                HttpContext.Session.SetInt32("IDEvaluacion", nuevaEvaluacion.EvaluacionID);

              
                return RedirectToAction("IniciarTestMCHART", "Preguntas");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al iniciar la evaluación: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpGet]
        public ActionResult IniciarTestMCHART()
        {
            var preguntas = _context.PreguntasMCHAT.ToList();
            ViewBag.Preguntas = preguntas;

            
            int? evaluacionId = HttpContext.Session.GetInt32("IDEvaluacion");

            if (evaluacionId.HasValue && evaluacionId.Value > 0)
            {
               
                var evaluacionExiste = _context.Evaluaciones.Any(e => e.EvaluacionID == evaluacionId.Value);

                if (evaluacionExiste)
                {
                   
                    var respuestasExistentes = _context.RespuestasMCHAT
                        .Where(r => r.EvaluacionID == evaluacionId.Value)
                        .ToDictionary(r => r.PreguntaID, r => r.Respuesta);

                    ViewBag.EvaluacionID = evaluacionId.Value;
                    ViewBag.RespuestasExistentes = respuestasExistentes;
                }
                else
                {
                    
                    HttpContext.Session.Remove("IDEvaluacion");
                    ViewBag.EvaluacionID = 0;
                    ViewBag.RespuestasExistentes = new Dictionary<int, bool>();
                }
            }
            else
            {
                ViewBag.EvaluacionID = 0;
                ViewBag.RespuestasExistentes = new Dictionary<int, bool>();
            }

            return View();
        }

        [HttpPost]
        public ActionResult CrearNuevaEvaluacion()
        {
            try
            {
                
                var nuevaEvaluacion = new Evaluacion
                {
                    FechaEvaluacion = DateTime.Now,
                    
                };

                _context.Evaluaciones.Add(nuevaEvaluacion);
                _context.SaveChanges();

                
                HttpContext.Session.SetInt32("IDEvaluacion", nuevaEvaluacion.EvaluacionID);

                return Json(new { success = true, evaluacionId = nuevaEvaluacion.EvaluacionID });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult IniciarTestMCHART([Bind("EvaluacionID, PreguntaID,Respuesta,FechaRespuesta")] RespuestaMCHAT respuesta_M, int pregunta_id)
        {
            try
            {
                
                var respuestaExistente = _context.RespuestasMCHAT
                    .FirstOrDefault(r => r.EvaluacionID == respuesta_M.EvaluacionID && r.PreguntaID == pregunta_id);

                if (respuestaExistente != null)
                {
                    
                    respuestaExistente.Respuesta = respuesta_M.Respuesta;
                    respuestaExistente.FechaRespuesta = DateTime.Now;
                    _context.Update(respuestaExistente);
                }
                else
                {
                    
                    respuesta_M.PreguntaID = pregunta_id;
                    respuesta_M.FechaRespuesta = DateTime.Now;
                    _context.RespuestasMCHAT.Add(respuesta_M);
                }

                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

     
        [HttpPost]
        public ActionResult FinalizarEvaluacion()
        {
            try
            {
                int evaluacionId = HttpContext.Session.GetInt32("IDEvaluacion") ?? 0;

                if (evaluacionId > 0)
                {
                    
                    var evaluacion = _context.Evaluaciones.Find(evaluacionId);
                    if (evaluacion != null)
                    {
                        
                        _context.SaveChanges();
                    }

                    // Limpiar sesión
                    HttpContext.Session.Remove("IDEvaluacion");
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        // GET: PreguntasController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PreguntasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PreguntasController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PreguntasController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // Métodos adicionales para el controlador PreguntasController para manejar ADI-R

        public ActionResult VerPreguntas_ADIR()
        {
            var preguntasConAreas = _context.PreguntasADIR
                .Join(_context.AreasADIR,
                      pregunta => pregunta.AreaID,
                      area => area.AreaID,
                      (pregunta, area) => new
                      {
                          PreguntaID = pregunta.PreguntaID,
                          AreaID = pregunta.AreaID,
                          NombreArea = area.NombreArea,
                          NumeroPregunta = pregunta.NumeroPregunta,
                          TextoPregunta = pregunta.TextoPregunta,
                          TipoPregunta = pregunta.TipoPregunta,
                          EsActiva = pregunta.EsActiva
                      })
                .ToList();

            return View(preguntasConAreas);
        }

        // GET: Crear pregunta ADI-R
        public ActionResult CreateADIR()
        {
            var areas = _context.AreasADIR.Where(a => a.EsActivo == true).ToList();
            ViewBag.Areas = areas;
            return View();
        }

        // POST: Crear pregunta ADI-R
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateADIR([Bind("PreguntaID,AreaID,NumeroPregunta,TextoPregunta,TipoPregunta,EsActiva")] PreguntaADIR pregunta_ADIR, int id_test)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Obtener el siguiente número de pregunta para el área específica
                    var ultimaPreguntaEnArea = _context.PreguntasADIR
                        .Where(p => p.AreaID == pregunta_ADIR.AreaID)
                        .OrderByDescending(p => p.NumeroPregunta)
                        .FirstOrDefault();

                    pregunta_ADIR.NumeroPregunta = ultimaPreguntaEnArea != null ? ultimaPreguntaEnArea.NumeroPregunta + 1 : 1;

                    _context.PreguntasADIR.Add(pregunta_ADIR);
                    _context.SaveChanges();

                    return RedirectToAction("CreateADIR", "Preguntas", new { id = id_test });
                }

                ViewBag.Areas = _context.AreasADIR.Where(a => a.EsActivo).ToList();
                return View(pregunta_ADIR);
            }
            catch
            {
                ViewBag.Areas = _context.AreasADIR.Where(a => a.EsActivo).ToList();
                return View();
            }
        }

        // GET: Editar preguntas ADI-R
        public ActionResult EditADIR()
        {
            var preguntasConAreas = _context.PreguntasADIR
                .Join(_context.AreasADIR,
                      pregunta => pregunta.AreaID,
                      area => area.AreaID,
                      (pregunta, area) => new
                      {
                          PreguntaID = pregunta.PreguntaID,
                          AreaID = pregunta.AreaID,
                          NombreArea = area.NombreArea,
                          NumeroPregunta = pregunta.NumeroPregunta,
                          TextoPregunta = pregunta.TextoPregunta,
                          TipoPregunta = pregunta.TipoPregunta,
                          EsActiva = pregunta.EsActiva
                      })
                .ToList();

            ViewBag.Areas = _context.AreasADIR.Where(a => a.EsActivo == true).ToList();

            return View(preguntasConAreas);
        }

        // POST: Editar pregunta ADI-R
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditADIR(PreguntaADIR model)
        {
            try
            {
                var preguntaExistente = _context.PreguntasADIR.FirstOrDefault(p => p.PreguntaID == model.PreguntaID);

                if (preguntaExistente == null)
                {
                    return Json(new { success = false, message = "Pregunta no encontrada." });
                }

                preguntaExistente.TextoPregunta = model.TextoPregunta;
                preguntaExistente.TipoPregunta = model.TipoPregunta;
                preguntaExistente.AreaID = model.AreaID;

                _context.SaveChanges();

                return RedirectToAction("EditADIR", "Preguntas");
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error al guardar la pregunta." });
            }
        }


        // Método auxiliar para obtener preguntas por área
        public ActionResult GetPreguntasPorArea(int areaId)
        {
            var preguntas = _context.PreguntasADIR
                .Where(p => p.AreaID == areaId && p.EsActiva)
                .OrderBy(p => p.NumeroPregunta)
                .ToList();

            return Json(preguntas);
        }

        public ActionResult VerPreguntas_ADOS2()
        {
            var preguntasConModulos = _context.PreguntasADOS2
                .Join(_context.ModulosADOS2,
                      pregunta => pregunta.ModuloID,
                      modulo => modulo.ModuloID,
                      (pregunta, modulo) => new
                      {
                          PreguntaID = pregunta.PreguntaID,
                          ModuloID = pregunta.ModuloID,
                          NombreModulo = modulo.NombreModulo,
                          NumeroPregunta = pregunta.NumeroPregunta,
                          TextoPregunta = pregunta.TextoPregunta,
                          TipoPregunta = pregunta.TipoPregunta,
                          EsActiva = pregunta.EsActiva
                      })
                .ToList();

            return View(preguntasConModulos);
        }

        // GET: Crear pregunta ADOS-2
        public ActionResult CreateADOS2()
        {
            var modulos = _context.ModulosADOS2.Where(m => m.EsActivo == true).ToList();
            ViewBag.Modulos = modulos;
            return View();
        }

        // POST: Crear pregunta ADOS-2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateADOS2([Bind("PreguntaID,ModuloID,NumeroPregunta,TextoPregunta,TipoPregunta,EsActiva")] PreguntaADOS2 pregunta_ADOS2, int id_test)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Obtener el siguiente número de pregunta para el módulo específico
                    var ultimaPreguntaEnModulo = _context.PreguntasADOS2
                        .Where(p => p.ModuloID == pregunta_ADOS2.ModuloID)
                        .OrderByDescending(p => p.NumeroPregunta)
                        .FirstOrDefault();

                    pregunta_ADOS2.NumeroPregunta = ultimaPreguntaEnModulo != null ? ultimaPreguntaEnModulo.NumeroPregunta + 1 : 1;

                    _context.PreguntasADOS2.Add(pregunta_ADOS2);
                    _context.SaveChanges();

                    return RedirectToAction("CreateADOS2", "Preguntas", new { id = id_test });
                }

                ViewBag.Modulos = _context.ModulosADOS2.Where(m => m.EsActivo).ToList();
                return View(pregunta_ADOS2);
            }
            catch
            {
                ViewBag.Modulos = _context.ModulosADOS2.Where(m => m.EsActivo).ToList();
                return View();
            }
        }

        // GET: Editar preguntas ADOS-2
        public ActionResult EditADOS2()
        {
            var preguntasConModulos = _context.PreguntasADOS2
                .Join(_context.ModulosADOS2,
                      pregunta => pregunta.ModuloID,
                      modulo => modulo.ModuloID,
                      (pregunta, modulo) => new
                      {
                          PreguntaID = pregunta.PreguntaID,
                          ModuloID = pregunta.ModuloID,
                          NombreModulo = modulo.NombreModulo,
                          NumeroPregunta = pregunta.NumeroPregunta,
                          TextoPregunta = pregunta.TextoPregunta,
                          TipoPregunta = pregunta.TipoPregunta,
                          EsActiva = pregunta.EsActiva
                      })
                .ToList();

            ViewBag.Modulos = _context.ModulosADOS2.Where(m => m.EsActivo == true).ToList();

            return View(preguntasConModulos);
        }

        // POST: Editar pregunta ADOS-2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditADOS2(PreguntaADOS2 model)
        {
            try
            {
                var preguntaExistente = _context.PreguntasADOS2.FirstOrDefault(p => p.PreguntaID == model.PreguntaID);

                if (preguntaExistente == null)
                {
                    return Json(new { success = false, message = "Pregunta no encontrada." });
                }

                preguntaExistente.TextoPregunta = model.TextoPregunta;
                preguntaExistente.TipoPregunta = model.TipoPregunta;
                preguntaExistente.ModuloID = model.ModuloID;

                _context.SaveChanges();

                return RedirectToAction("EditADOS2", "Preguntas");
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error al guardar la pregunta." });
            }
        }

        // Método auxiliar para obtener preguntas por módulo
        public ActionResult GetPreguntasPorModulo(int moduloId)
        {
            var preguntas = _context.PreguntasADOS2
                .Where(p => p.ModuloID == moduloId && p.EsActiva)
                .OrderBy(p => p.NumeroPregunta)
                .ToList();

            return Json(preguntas);
        }
    }
}
