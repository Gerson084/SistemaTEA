using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTEA.Models;

namespace SistemaTEA.Controllers
{
    public class EvaluacionController : Controller
    {

        private readonly EvaluacionContext _context;
        public EvaluacionController(EvaluacionContext context)
        {
            _context = context;
        }


        // Ejemplo en tu controlador o servicio
        public IActionResult CrearEvaluacionADOS(int pacienteId)
        {
            var nuevaEvaluacion = new Evaluacion
            {
                PacienteID = 1,
                FechaEvaluacion = DateTime.Now,
                TipoTestID=1,
                UsuarioEvaluadorID=4,
                // Otros datos como profesional o módulo
            };
            _context.Evaluaciones.Add(nuevaEvaluacion);
            _context.SaveChanges();

            // Rediriges a IniciarEvaluacion con el ID que se acaba de generar
            return View("~/Views/ADOS/IniciarEvaluacion.cshtml", nuevaEvaluacion);


        }
        public IActionResult IniciarEvaluacion(int evaluacionId)
        {
            // Aquí cargas la evaluación y pasas la vista correspondiente
            var evaluacion = _context.Evaluaciones
                .Include(e => e.Paciente)
                .FirstOrDefault(e => e.EvaluacionID == evaluacionId);

            if (evaluacion == null)
                return NotFound();

            return View(evaluacion); // Asegúrate de tener una vista IniciarEvaluacion.cshtml
        }





        // GET: EvaluacionController
        public ActionResult Index()
        {
            return View();
        }

        // GET: EvaluacionController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EvaluacionController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EvaluacionController/Create
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

        // GET: EvaluacionController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EvaluacionController/Edit/5
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

        // GET: EvaluacionController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EvaluacionController/Delete/5
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
