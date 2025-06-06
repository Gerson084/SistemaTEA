using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTEA.Models;
using SistemaTEA.ViewModels;

namespace SistemaTEA.Controllers
{
    public class EvaluacionController : Controller
    {

        private readonly EvaluacionContext _context;
        public EvaluacionController(EvaluacionContext context)
        {
            _context = context;
        }


        // GET: Cargar preguntas en la vista
        public IActionResult ModuloT(int moduloId)
        {
            var preguntas = _context.PreguntasADOS2
                .Where(p => p.ModuloID == moduloId)
                .OrderBy(p => p.NumeroPregunta)
                .ToList();

            var viewModel = new ADOSEvaluacionViewModel
            {
                ModuloID = moduloId,
                Preguntas = preguntas
            };

            return View(viewModel);
        }

        // POST: Recibir respuestas y guardarlas
        [HttpPost]
        public IActionResult GuardarEvaluacion(int moduloId, [FromForm] Dictionary<int, string> Respuestas)
        {
            // Aquí asumo que tienes una entidad RespuestaADOS2 con al menos PreguntaID y TextoRespuesta
            foreach (var item in Respuestas)
            {
                var nuevaRespuesta = new RespuestaADOS2
                {
                    PreguntaID = item.Key,
                    
                    // Agrega otros campos necesarios (Ejemplo: Fecha, UsuarioID, etc.)
                };

                _context.RespuestasADOS2.Add(nuevaRespuesta);
            }

            _context.SaveChanges();

            // Redirige a una vista de confirmación o donde quieras
            return RedirectToAction("Confirmacion");
        }

        public IActionResult Confirmacion()
        {
            return View();
        }

        public IActionResult BuscarPaciente()
        {
            return View("~/Views/ADOS/BuscarPaciente.cshtml");
        }

        [HttpPost]
        public IActionResult BuscarPaciente(string nombrePaciente)
        {
            var paciente = _context.Pacientes
                .FirstOrDefault(p => (p.Nombre + " " + p.Apellido).Contains(nombrePaciente));

            if (paciente == null)
            {
                ViewBag.Mensaje = "Paciente no encontrado.";
                return View("~/Views/ADOS/BuscarPaciente.cshtml");
            }

            // Calcular edad en años y meses
            int edadEnMeses = ((DateTime.Now.Year - paciente.FechaNacimiento.Year) * 12) +
                              (DateTime.Now.Month - paciente.FechaNacimiento.Month);

            int edadEnAnios = DateTime.Now.Year - paciente.FechaNacimiento.Year;
            if (DateTime.Now.Date < paciente.FechaNacimiento.Date.AddYears(edadEnAnios))
                edadEnAnios--;

            // Sugerir módulo únicamente por edad
            string moduloSugerido = "";

            if (edadEnMeses >= 12 && edadEnMeses <= 30)
            {
                moduloSugerido = "Módulo T";
            }
            else if (edadEnMeses >= 31 && edadEnAnios < 16)
            {
                moduloSugerido = "Módulo 1, 2 o 3 (según lenguaje)";
            }
            else if (edadEnAnios >= 16)
            {
                moduloSugerido = "Módulo 4";
            }
            else
            {
                moduloSugerido = "No hay módulo sugerido para esta edad.";
            }

            ViewBag.ModuloSugerido = moduloSugerido;

            return View("~/Views/ADOS/BuscarPaciente.cshtml", paciente);
        }



        [HttpPost]
        public IActionResult IniciarEvaluacion(int pacienteId, string moduloSeleccionado)
        {
            var nuevaEvaluacion = new Evaluacion
            {
                PacienteID = pacienteId,
                FechaEvaluacion = DateTime.Now,
                TipoTestID = 1,
                UsuarioEvaluadorID = ObtenerUsuarioActual(),
            };

            _context.Evaluaciones.Add(nuevaEvaluacion);
            _context.SaveChanges();

            // Redirige según el módulo seleccionado
            switch (moduloSeleccionado)
            {
                case "T":
                    return RedirectToAction("ModuloT", "ADOS", new { evaluacionId = nuevaEvaluacion.EvaluacionID });
                case "1":
                    return RedirectToAction("Modulo1", "ADOS", new { evaluacionId = nuevaEvaluacion.EvaluacionID });
                case "2":
                    return RedirectToAction("Modulo2", "ADOS", new { evaluacionId = nuevaEvaluacion.EvaluacionID });
                case "3":
                    return RedirectToAction("Modulo3", "ADOS", new { evaluacionId = nuevaEvaluacion.EvaluacionID });
                case "4":
                    return RedirectToAction("Modulo4", "ADOS", new { evaluacionId = nuevaEvaluacion.EvaluacionID });
                default:
                    TempData["Error"] = "Debe seleccionar un módulo válido.";
                    return RedirectToAction("BuscarPaciente");
            }
        }

        

        private int ObtenerUsuarioActual()
        {
            var userId = HttpContext.Session.GetInt32("id_usuario");
            if (userId == null)
                throw new Exception("Usuario no autenticado.");
            return userId.Value;
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
