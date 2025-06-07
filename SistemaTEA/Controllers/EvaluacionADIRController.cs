using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTEA.Models;
using SistemaTEA.ViewModels;

namespace SistemaTEA.Controllers
{
    public class EvaluacionADIRController : Controller
    {

        private readonly EvaluacionContext _context;
        public EvaluacionADIRController(EvaluacionContext context)
        {
            _context = context;
        }

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

        [HttpPost]
        public IActionResult GuardarEvaluacion(int moduloId, [FromForm] Dictionary<int, string> Respuestas)
        {
            foreach (var item in Respuestas)
            {
                var nuevaRespuesta = new RespuestaADIR
                {
                    PreguntaID = item.Key,

                };

                _context.RespuestasADIR.Add(nuevaRespuesta);
            }

            _context.SaveChanges();


            return RedirectToAction("Confirmacion");
        }

        public IActionResult Confirmacion()
        {
            return View();
        }

        public IActionResult BuscarPacienteADIR()
        {
            return View("~/Views/ADIR/BuscarPacienteADIR.cshtml");
        }
                                     

        [HttpPost]
        public IActionResult BuscarPacienteADIR(string nombrePaciente)
        {
            int psicologoId = ObtenerUsuarioActual();

            var paciente = _context.Pacientes
                .FirstOrDefault(p =>
                    (p.Nombre + " " + p.Apellido).Contains(nombrePaciente) &&
                    p.PsicologoAsignadoID == psicologoId);

            if (paciente == null)
            {
                TempData["Error"] = "No se encontró ningún paciente con ese nombre.";
                return View("~/Views/ADIR/BuscarPacienteADIR.cshtml");
            }


            var evaluacionEnProceso = _context.Evaluaciones
                .FirstOrDefault(e =>
                    e.PacienteID == paciente.PacienteID &&
                    e.EstadoEvaluacion == "En Proceso");

            if (evaluacionEnProceso != null)
            {
                ViewBag.EvaluacionEnProceso = true;
                ViewBag.MensajeEvaluacion = "Este paciente ya tiene una evaluación en proceso.";
                ViewBag.EvaluacionID = evaluacionEnProceso.EvaluacionID;
            }
            else
            {
                ViewBag.EvaluacionEnProceso = false;
            }

            int edadEnMeses = ((DateTime.Now.Year - paciente.FechaNacimiento.Year) * 12) +
                              (DateTime.Now.Month - paciente.FechaNacimiento.Month);

            int edadEnAnios = DateTime.Now.Year - paciente.FechaNacimiento.Year;
            if (DateTime.Now.Date < paciente.FechaNacimiento.Date.AddYears(edadEnAnios))
                edadEnAnios--;

           
           

            return View("~/Views/ADIR/BuscarPacienteADIR.cshtml", paciente);
        }



        [HttpGet]
        public IActionResult IniciarEvaluacionADIR(int pacienteId)
        {
            // Aquí mostrar vista o redirigir
            // Puedes mostrar un formulario para seleccionar módulo o directamente crear la evaluación
            return View();
        }





        [HttpPost]
        public IActionResult IniciarEvaluacionADIR(int pacienteId, string areaSeleccionado)
        {
            // Determinar el ID del módulo según el valor seleccionado
            int? moduloId = areaSeleccionado switch
            {
                "1" => 1,
                "2" => 2,
                "3" => 3,
                "4" => 4,
                "5" => 5,
                "6" => 6,
                "7" => 7,
                _ => null
            };

            if (moduloId == null)
            {
                TempData["Error"] = "Debe seleccionar un módulo válido.";
                return RedirectToAction("BuscarPacienteADIR");
            }


            var evaluacionExistente = _context.Evaluaciones
                .FirstOrDefault(e => e.PacienteID == pacienteId
                                  && e.TipoTestID == 3 // ADOS
                                  && e.EstadoEvaluacion == "En Proceso");

            if (evaluacionExistente != null)
            {

                return RedirectToAction("Area" + areaSeleccionado, "ADIR", new { evaluacionId = evaluacionExistente.EvaluacionID });
            }


            var nuevaEvaluacion = new Evaluacion
            {
                PacienteID = pacienteId,
                FechaEvaluacion = DateTime.Now,
                TipoTestID = 3, // ADOS
                UsuarioEvaluadorID = ObtenerUsuarioActual(),
                ModuloADOS2ID = moduloId,
                EstadoEvaluacion = "En Proceso"
            };

            _context.Evaluaciones.Add(nuevaEvaluacion);
            _context.SaveChanges();


            switch (areaSeleccionado)
            {
                case "1":
                    return RedirectToAction("Area1", "ADIR", new { evaluacionId = nuevaEvaluacion.EvaluacionID });
                case "2":
                    return RedirectToAction("Area2", "ADIR", new { evaluacionId = nuevaEvaluacion.EvaluacionID });
                case "3":
                    return RedirectToAction("Area3", "ADIR", new { evaluacionId = nuevaEvaluacion.EvaluacionID });
                case "4":
                    return RedirectToAction("Area4", "ADIR", new { evaluacionId = nuevaEvaluacion.EvaluacionID });
                case "5":
                    return RedirectToAction("Area5", "ADIR", new { evaluacionId = nuevaEvaluacion.EvaluacionID });
                case "6":
                    return RedirectToAction("Area6", "ADIR", new { evaluacionId = nuevaEvaluacion.EvaluacionID });
                case "7":
                    return RedirectToAction("Area7", "ADIR", new { evaluacionId = nuevaEvaluacion.EvaluacionID });
                default:
                    TempData["Error"] = "Debe seleccionar un módulo válido.";
                    return RedirectToAction("BuscarPacienteADIR");
            }
        }




        private int ObtenerUsuarioActual()
        {
            var userId = HttpContext.Session.GetInt32("UsuarioID");
            if (userId == null)
                throw new Exception("Usuario no autenticado.");
            return userId.Value;
        }









        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Details(int id)
        {
            return View();
        }


        public ActionResult Create()
        {
            return View();
        }


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


        public ActionResult Edit(int id)
        {
            return View();
        }


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


        public ActionResult Delete(int id)
        {
            return View();
        }


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

        [HttpPost]
        public IActionResult ContinuarEvaluacion(int evaluacionId)
        {
            var evaluacion = _context.Evaluaciones
                .FirstOrDefault(e => e.EvaluacionID == evaluacionId);

            if (evaluacion == null)
            {
                TempData["Error"] = "No se encontró la evaluación.";
                return RedirectToAction("BuscarPacienteADIR", "ADIR");
            }


            string moduloSeleccionado = evaluacion.ModuloADOS2ID switch
            {
                1 => "1",
                2 => "2",
                3 => "3",
                4 => "4",
                5 => "5",
                6 => "6",
                7 => "7",
                8 => "8",
                _ => null
            };

            if (moduloSeleccionado == null)
            {
                TempData["Error"] = "Area inválida.";
                return RedirectToAction("BuscarPacienteADIR", "ADIR");
            }


            return RedirectToAction("Area" + moduloSeleccionado, "ADIR", new { evaluacionId = evaluacion.EvaluacionID });
        }

        public IActionResult ResultadoFinal(int evaluacionId)
        {

            var notaTotal = _context.RespuestasADOS2
                .Where(r => r.EvaluacionID == evaluacionId)
                .Sum(r => (int?)r.Puntuacion) ?? 0;

            ViewBag.EvaluacionID = evaluacionId;
            ViewBag.NotaTotal = notaTotal;

            return View("~/Views/ADIR/ResultadoFinalADIR.cshtml");
        }




        [HttpPost]
        public IActionResult TerminarEvaluacion(int EvaluacionID)
        {
            try
            {

                var notaTotal = _context.RespuestasADIR
                    .Where(r => r.EvaluacionID == EvaluacionID)
                    .Sum(r => (int?)r.Puntuacion) ?? 0;


                var evaluacion = _context.Evaluaciones.FirstOrDefault(e => e.EvaluacionID == EvaluacionID);
                if (evaluacion == null)
                {
                    return NotFound("Evaluación no encontrada.");
                }


                evaluacion.EstadoEvaluacion = "Completado";
                evaluacion.PuntajeTotal = notaTotal;

                _context.SaveChanges();


                return RedirectToAction("ResultadoFinal", new { evaluacionId = EvaluacionID, nota = notaTotal });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al finalizar la evaluación: {ex.Message}" });
            }
        }








    }
}
