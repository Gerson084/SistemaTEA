using Microsoft.AspNetCore.Http;
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
            ViewBag.Areas = _context.AreasADIR.Where(a => a.EsActivo).ToList();
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

        // GET: Eliminar pregunta ADI-R
        public ActionResult DeleteADIR(int id)
        {
            var pregunta = _context.PreguntasADIR
                .Join(_context.AreasADIR,
                      p => p.AreaID,
                      a => a.AreaID,
                      (p, a) => new
                      {
                          PreguntaID = p.PreguntaID,
                          AreaID = p.AreaID,
                          NombreArea = a.NombreArea,
                          NumeroPregunta = p.NumeroPregunta,
                          TextoPregunta = p.TextoPregunta,
                          TipoPregunta = p.TipoPregunta,
                          EsActiva = p.EsActiva
                      })
                .FirstOrDefault(p => p.PreguntaID == id);

            if (pregunta == null)
            {
                return NotFound();
            }

            return View(pregunta);
        }

        // POST: Eliminar pregunta ADI-R
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteADIR(int id, IFormCollection collection)
        {
            try
            {
                var pregunta = _context.PreguntasADIR.FirstOrDefault(p => p.PreguntaID == id);

                if (pregunta != null)
                {
                    _context.PreguntasADIR.Remove(pregunta);
                    _context.SaveChanges();
                }

                return RedirectToAction("VerPreguntas_ADIR");
            }
            catch
            {
                return View();
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
    }
}
