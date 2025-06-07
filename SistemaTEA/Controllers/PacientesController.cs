using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaTEA.Models;

namespace SistemaTEA.Controllers
{
    public class PacientesController : Controller
    {
        private readonly EvaluacionContext _context;

        public PacientesController(EvaluacionContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var padreID = HttpContext.Session.GetInt32("UsuarioID");
            if (padreID == null)
            {
                TempData["MensajeError"] = "Sesión expirada. Por favor inicie sesión nuevamente.";
                return RedirectToAction("Login", "Cuenta");
            }

            var padre = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UsuarioID == padreID && u.RolID == 2 && u.EsActivo == true);

            if (padre == null)
            {
                TempData["MensajeError"] = "Usuario no válido o no activo.";
                return RedirectToAction("Login", "Cuenta");
            }

            var pacientes = await _context.Pacientes
                .Include(p => p.PsicologoAsignado)
                .Where(p => p.PadreID == padreID)
                .OrderBy(p => p.Nombre)
                .ThenBy(p => p.Apellido)
                .ToListAsync();

            ViewBag.NombrePadre = padre.Nombre;
            return View(pacientes);
        }

        public IActionResult Consentimiento()
        {
            var usuarioID = HttpContext.Session.GetInt32("UsuarioID");
            if (usuarioID == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var usuario = _context.Usuarios.FirstOrDefault(u => u.UsuarioID == usuarioID && u.RolID == 2 && u.EsActivo == true);
            if (usuario == null)
            {
                TempData["MensajeError"] = "Usuario no válido.";
                return RedirectToAction("Login", "Cuenta");
            }

            return View();
        }


        [HttpPost]
        public IActionResult Consentimiento(string consentimiento)
        {
            if (consentimiento == "true")
            {

                HttpContext.Session.SetString("ConsentimientoOtorgado", "true");
                HttpContext.Session.SetString("FechaConsentimiento", DateTime.Now.ToString());
                HttpContext.Session.SetString("IPConsentimiento", GetClientIP());


                return RedirectToAction("Crear");
            }
            else
            {
                TempData["MensajeInfo"] = "Es necesario aceptar el consentimiento para registrar un paciente.";
                return RedirectToAction("Index"); // o Dashboard
            }
        }


        private string GetClientIP()
        {
            var xForwarded = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(xForwarded))
            {
                return xForwarded.Split(',')[0].Trim();
            }

            var xRealIP = Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(xRealIP))
            {
                return xRealIP;
            }

            return Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }


        public IActionResult Crear()
        {
            var consentimientoOtorgado = HttpContext.Session.GetString("ConsentimientoOtorgado");
            if (consentimientoOtorgado != "true")
            {
                return RedirectToAction("Consentimiento");
            }

            ViewBag.Psicologos = new SelectList(
                _context.Usuarios.Where(u => u.RolID == 3 && u.EsActivo == true),
                "UsuarioID",
                "Nombre"
            );
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Paciente paciente)
        {
            try
            {
                var consentimientoOtorgado = HttpContext.Session.GetString("ConsentimientoOtorgado");
                if (consentimientoOtorgado != "true")
                {
                    TempData["MensajeError"] = "Es necesario aceptar el consentimiento para registrar un paciente.";
                    return RedirectToAction("Consentimiento");
                }

                Console.WriteLine($"Datos recibidos - Nombre: {paciente.Nombre}, Apellido: {paciente.Apellido}, Fecha: {paciente.FechaNacimiento}, Género: {paciente.Genero}");

                var padreID = HttpContext.Session.GetInt32("UsuarioID");
                if (padreID == null)
                {
                    TempData["MensajeError"] = "Sesión expirada. Por favor inicie sesión nuevamente.";
                    return RedirectToAction("Login", "Cuenta");
                }

                Console.WriteLine($"PadreID de sesión: {padreID}");

                var padre = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.UsuarioID == padreID && u.RolID == 2 && u.EsActivo == true);

                if (padre == null)
                {
                    TempData["MensajeError"] = "Usuario no válido o no activo.";
                    return RedirectToAction("Login", "Cuenta");
                }

                if (paciente.PsicologoAsignadoID.HasValue)
                {
                    var psicologo = await _context.Usuarios
                        .FirstOrDefaultAsync(u => u.UsuarioID == paciente.PsicologoAsignadoID &&
                                                 u.RolID == 3 && u.EsActivo == true);

                    if (psicologo == null)
                    {
                        ModelState.AddModelError("PsicologoAsignadoID", "El psicólogo seleccionado no es válido.");
                    }
                }

                if (paciente.FechaNacimiento > DateTime.Now.Date)
                {
                    ModelState.AddModelError("FechaNacimiento", "La fecha de nacimiento no puede ser futura.");
                }

                if (!string.IsNullOrEmpty(paciente.Genero) &&
                    paciente.Genero.ToUpper() != "M" && paciente.Genero.ToUpper() != "F")
                {
                    ModelState.AddModelError("Genero", "El género debe ser 'M' o 'F'.");
                }

                if (ModelState.IsValid)
                {
                    var fechaConsentimiento = DateTime.TryParse(HttpContext.Session.GetString("FechaConsentimiento"), out DateTime fecha) ? fecha : DateTime.Now;
                    var ipConsentimiento = HttpContext.Session.GetString("IPConsentimiento") ?? GetClientIP();

                    var nuevoPaciente = new Paciente
                    {
                        Nombre = paciente.Nombre?.Trim(),
                        Apellido = paciente.Apellido?.Trim(),
                        FechaNacimiento = paciente.FechaNacimiento,
                        Genero = paciente.Genero?.ToUpper().Trim(),
                        PadreID = padreID.Value,
                        PsicologoAsignadoID = paciente.PsicologoAsignadoID,
                        FechaRegistro = DateTime.Now,
                        Observaciones = !string.IsNullOrEmpty(paciente.Observaciones) ? paciente.Observaciones.Trim() : null,

                        ConsentimientoOtorgado = true,
                        FechaConsentimiento = fechaConsentimiento,
                        IPConsentimiento = ipConsentimiento
                    };


                    _context.Pacientes.Add(nuevoPaciente);


                    Console.WriteLine($"Guardando - Nombre: {nuevoPaciente.Nombre}, Apellido: {nuevoPaciente.Apellido}, PadreID: {nuevoPaciente.PadreID}");

                    // Guardar cambios
                    var resultado = await _context.SaveChangesAsync();

                    Console.WriteLine($"Registros afectados: {resultado}");


                    HttpContext.Session.Remove("ConsentimientoOtorgado");
                    HttpContext.Session.Remove("FechaConsentimiento");
                    HttpContext.Session.Remove("IPConsentimiento");

                    TempData["MensajeExito"] = $"Paciente {paciente.Nombre} {paciente.Apellido} registrado con éxito.";
                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateException dbEx)
            {

                TempData["MensajeError"] = "Error al guardar en la base de datos. Verifique que todos los datos sean correctos.";

                Console.WriteLine($"DbUpdateException: {dbEx.Message}");
                if (dbEx.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {dbEx.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                // Error general
                TempData["MensajeError"] = "Ocurrió un error inesperado. Por favor intente nuevamente.";
                Console.WriteLine($"Exception: {ex.Message}");
            }


            ViewBag.Psicologos = new SelectList(
                _context.Usuarios.Where(u => u.RolID == 3 && u.EsActivo == true),
                "UsuarioID",
                "Nombre",
                paciente.PsicologoAsignadoID
            );

            return View(paciente);
        }


        public async Task<IActionResult> Lista()
        {
            var padreID = HttpContext.Session.GetInt32("UsuarioID");
            if (padreID == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var pacientes = await _context.Pacientes
                .Include(p => p.PsicologoAsignado)
                .Where(p => p.PadreID == padreID)
                .OrderBy(p => p.Nombre)
                .ToListAsync();

            return View(pacientes);
        }


        public async Task<IActionResult> VerTodos()
        {

            var usuarioID = HttpContext.Session.GetInt32("UsuarioID");
            var rol = HttpContext.Session.GetInt32("rol");

            if (usuarioID == null)
            {
                TempData["MensajeError"] = "Sesión expirada. Por favor inicie sesión nuevamente.";
                return RedirectToAction("Login", "Login");
            }


            if (rol != 1 && rol != 3)
            {
                TempData["MensajeError"] = "No tiene permisos para ver esta información.";
                return RedirectToAction("Index", "Home");
            }


            var pacientes = await _context.Pacientes
                .Include(p => p.Padre)
                .Include(p => p.PsicologoAsignado)
                .Select(p => new
                {
                    PacienteID = p.PacienteID,
                    Nombre = p.Nombre,
                    Apellido = p.Apellido,
                    FechaNacimiento = p.FechaNacimiento,
                    Genero = p.Genero,
                    PadreID = p.PadreID,
                    NombrePadre = p.Padre.Nombre + " " + p.Padre.Apellido,
                    EmailPadre = p.Padre.Email,
                    TelefonoPadre = p.Padre.Telefono,
                    PsicologoAsignadoID = p.PsicologoAsignadoID,
                    NombrePsicologo = p.PsicologoAsignado != null ?
                                    p.PsicologoAsignado.Nombre + " " + p.PsicologoAsignado.Apellido : "No asignado",
                    EmailPsicologo = p.PsicologoAsignado != null ? p.PsicologoAsignado.Email : "",
                    FechaRegistro = p.FechaRegistro,
                    Observaciones = p.Observaciones,
                    Edad = DateTime.Now.Year - p.FechaNacimiento.Year -
                           (DateTime.Now.DayOfYear < p.FechaNacimiento.DayOfYear ? 1 : 0)
                })
                .OrderBy(p => p.Nombre)
                .ThenBy(p => p.Apellido)
                .ToListAsync();

            var psicologos = await _context.Usuarios
                .Where(u => u.RolID == 3 && u.EsActivo == true)
                .Select(u => new { u.UsuarioID, NombreCompleto = u.Nombre + " " + u.Apellido })
                .OrderBy(u => u.NombreCompleto)
                .ToListAsync();


            ViewBag.NombreUsuario = HttpContext.Session.GetString("nombre") ?? "Usuario";
            ViewData["rol"] = rol;
            ViewBag.TotalPacientes = pacientes.Count();
            ViewBag.PacientesConPsicologo = pacientes.Count(p => p.PsicologoAsignadoID.HasValue);
            ViewBag.PacientesSinPsicologo = pacientes.Count(p => !p.PsicologoAsignadoID.HasValue);

            ViewBag.Psicologos = psicologos;

            return View(pacientes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarPsicologo(int pacienteId, int psicologoId)
        {
            try
            {

                var usuarioID = HttpContext.Session.GetInt32("UsuarioID");
                var rol = HttpContext.Session.GetInt32("rol");

                if (usuarioID == null || (rol != 1 && rol != 3))
                {
                    return Json(new { success = false, message = "No tiene permisos para realizar esta acción." });
                }


                var paciente = await _context.Pacientes.FindAsync(pacienteId);
                if (paciente == null)
                {
                    return Json(new { success = false, message = "Paciente no encontrado." });
                }


                var psicologo = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.UsuarioID == psicologoId && u.RolID == 3 && u.EsActivo == true);

                if (psicologo == null)
                {
                    return Json(new { success = false, message = "Psicólogo no válido o inactivo." });
                }


                paciente.PsicologoAsignadoID = psicologoId;


                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = $"Psicólogo {psicologo.Nombre} {psicologo.Apellido} asignado correctamente al paciente {paciente.Nombre} {paciente.Apellido}.",
                    psicologoNombre = $"{psicologo.Nombre} {psicologo.Apellido}",
                    psicologoEmail = psicologo.Email
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al asignar psicólogo: {ex.Message}");
                return Json(new { success = false, message = "Error interno del servidor. Intente nuevamente." });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverPsicologo(int pacienteId)
        {
            try
            {

                var usuarioID = HttpContext.Session.GetInt32("UsuarioID");
                var rol = HttpContext.Session.GetInt32("rol");

                if (usuarioID == null || (rol != 1 && rol != 3))
                {
                    return Json(new { success = false, message = "No tiene permisos para realizar esta acción." });
                }


                var paciente = await _context.Pacientes.FindAsync(pacienteId);
                if (paciente == null)
                {
                    return Json(new { success = false, message = "Paciente no encontrado." });
                }


                paciente.PsicologoAsignadoID = null;

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = $"Psicólogo removido correctamente del paciente {paciente.Nombre} {paciente.Apellido}."
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al remover psicólogo: {ex.Message}");
                return Json(new { success = false, message = "Error interno del servidor. Intente nuevamente." });
            }
        }
    }
}