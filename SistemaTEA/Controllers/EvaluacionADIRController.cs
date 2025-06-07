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
                var nuevaRespuesta = new RespuestaADIR
                {
                    PreguntaID = item.Key,

                    // Agrega otros campos necesarios (Ejemplo: Fecha, UsuarioID, etc.)
                };

                _context.RespuestasADIR.Add(nuevaRespuesta);
            }

            _context.SaveChanges();

            // Redirige a una vista de confirmación o donde quieras
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

            // Verificar si tiene una evaluación en proceso
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

            // Calcular edad
            int edadEnMeses = ((DateTime.Now.Year - paciente.FechaNacimiento.Year) * 12) +
                              (DateTime.Now.Month - paciente.FechaNacimiento.Month);

            int edadEnAnios = DateTime.Now.Year - paciente.FechaNacimiento.Year;
            if (DateTime.Now.Date < paciente.FechaNacimiento.Date.AddYears(edadEnAnios))
                edadEnAnios--;

           
           

            return View("~/Views/ADIR/BuscarPacienteADIR.cshtml", paciente);
        }


        /* [HttpPost]
         public IActionResult BuscarPaciente(string nombrePaciente)
         {
             int psicologoId = ObtenerUsuarioActual();

             var paciente = _context.Pacientes
                 .FirstOrDefault(p =>
                     (p.Nombre + " " + p.Apellido).Contains(nombrePaciente) &&
                     p.PsicologoAsignadoID == psicologoId);

             if (paciente == null)
             {
                 ViewBag.Mensaje = "Paciente no encontrado o no está asignado a usted.";
                 TempData["Error"] = "No se encontró ningún paciente con ese nombre.";
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
         }*/

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

            // Verificar si ya hay una evaluación "En Proceso" para este paciente, tipo ADOS
            var evaluacionExistente = _context.Evaluaciones
                .FirstOrDefault(e => e.PacienteID == pacienteId
                                  && e.TipoTestID == 3 // ADOS
                                  && e.EstadoEvaluacion == "En Proceso");

            if (evaluacionExistente != null)
            {
                // Ya tiene una evaluación en proceso, redirigir a esa
                return RedirectToAction("Area" + areaSeleccionado, "ADIR", new { evaluacionId = evaluacionExistente.EvaluacionID });
            }

            // Si no hay evaluación en proceso, crear una nueva
            var nuevaEvaluacion = new Evaluacion
            {
                PacienteID = pacienteId,
                FechaEvaluacion = DateTime.Now,
                TipoTestID = 3, // ADOS
                UsuarioEvaluadorID = ObtenerUsuarioActual(),
                AreaADIRID = moduloId,
                EstadoEvaluacion = "En Proceso"
            };

            _context.Evaluaciones.Add(nuevaEvaluacion);
            _context.SaveChanges();

            // Redirigir al área correspondiente con el ID de la nueva evaluación
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





        /*[HttpPost]
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
        */


        private int ObtenerUsuarioActual()
        {
            var userId = HttpContext.Session.GetInt32("UsuarioID");
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

            // Mapear moduloId a su código de módulo para la redirección
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

            // Redirigir a la acción del módulo con evaluacionId
            return RedirectToAction("Area" + moduloSeleccionado, "ADIR", new { evaluacionId = evaluacion.EvaluacionID });
        }

        public IActionResult ResultadoFinal(int evaluacionId)
        {
            // Consultar la nota segura desde la BD
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
                // Calcular la nota total de esta evaluación
                var notaTotal = _context.RespuestasADIR
                    .Where(r => r.EvaluacionID == EvaluacionID)
                    .Sum(r => (int?)r.Puntuacion) ?? 0;

                // Buscar la evaluación en la base de datos
                var evaluacion = _context.Evaluaciones.FirstOrDefault(e => e.EvaluacionID == EvaluacionID);
                if (evaluacion == null)
                {
                    return NotFound("Evaluación no encontrada.");
                }

                // Actualizar los campos: EstadoEvaluacion y PuntajeTotal
                evaluacion.EstadoEvaluacion = "Completado";
                evaluacion.PuntajeTotal = notaTotal;

                _context.SaveChanges();

                // Redirigir a una vista con el resultado
                return RedirectToAction("ResultadoFinal", new { evaluacionId = EvaluacionID, nota = notaTotal });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al finalizar la evaluación: {ex.Message}" });
            }
        }








    }
}
