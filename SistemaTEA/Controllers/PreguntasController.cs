using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    }
}
