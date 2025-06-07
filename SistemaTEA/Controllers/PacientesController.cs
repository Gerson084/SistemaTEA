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
            // Verificar consentimiento
            var consentimientoOtorgado = HttpContext.Session.GetString("ConsentimientoOtorgado");
            if (consentimientoOtorgado != "true")
            {
                // Redirigir a la página de consentimiento
                return RedirectToAction("Consentimiento");
            }

            // Cargar lista de psicólogos para el ComboBox (RolID = 3)
            ViewBag.Psicologos = new SelectList(
                _context.Usuarios.Where(u => u.RolID == 3 && u.EsActivo == true),
                "UsuarioID",
                "Nombre"
            );
            return View();
        }

        // POST: Pacientes/Crear - MODIFICADO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Paciente paciente)
        {
            try
            {
                // Verificar consentimiento
                var consentimientoOtorgado = HttpContext.Session.GetString("ConsentimientoOtorgado");
                if (consentimientoOtorgado != "true")
                {
                    TempData["MensajeError"] = "Es necesario aceptar el consentimiento para registrar un paciente.";
                    return RedirectToAction("Consentimiento");
                }

                // Debug: Verificar qué datos llegan
                Console.WriteLine($"Datos recibidos - Nombre: {paciente.Nombre}, Apellido: {paciente.Apellido}, Fecha: {paciente.FechaNacimiento}, Género: {paciente.Genero}");

                // Obtener el ID del padre desde la sesión
                var padreID = HttpContext.Session.GetInt32("UsuarioID");
                if (padreID == null)
                {
                    TempData["MensajeError"] = "Sesión expirada. Por favor inicie sesión nuevamente.";
                    return RedirectToAction("Login", "Cuenta");
                }

                Console.WriteLine($"PadreID de sesión: {padreID}");

                // Verificar que el padre existe y tiene el rol correcto
                var padre = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.UsuarioID == padreID && u.RolID == 2 && u.EsActivo == true);

                if (padre == null)
                {
                    TempData["MensajeError"] = "Usuario no válido o no activo.";
                    return RedirectToAction("Login", "Cuenta");
                }

                // Verificar que el psicólogo asignado existe (si se seleccionó uno)
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

                // Validar que la fecha de nacimiento no sea futura
                if (paciente.FechaNacimiento > DateTime.Now.Date)
                {
                    ModelState.AddModelError("FechaNacimiento", "La fecha de nacimiento no puede ser futura.");
                }

                // Validar que el género sea válido
                if (!string.IsNullOrEmpty(paciente.Genero) &&
                    paciente.Genero.ToUpper() != "M" && paciente.Genero.ToUpper() != "F")
                {
                    ModelState.AddModelError("Genero", "El género debe ser 'M' o 'F'.");
                }

                if (ModelState.IsValid)
                {
                    // Obtener datos de consentimiento de la sesión
                    var fechaConsentimiento = DateTime.TryParse(HttpContext.Session.GetString("FechaConsentimiento"), out DateTime fecha) ? fecha : DateTime.Now;
                    var ipConsentimiento = HttpContext.Session.GetString("IPConsentimiento") ?? GetClientIP();

                    // Limpiar y asignar valores correctamente
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

                        // Campos de consentimiento
                        ConsentimientoOtorgado = true,
                        FechaConsentimiento = fechaConsentimiento,
                        IPConsentimiento = ipConsentimiento
                    };

                    // Agregar al contexto
                    _context.Pacientes.Add(nuevoPaciente);

                    // Debug: Verificar antes de guardar
                    Console.WriteLine($"Guardando - Nombre: {nuevoPaciente.Nombre}, Apellido: {nuevoPaciente.Apellido}, PadreID: {nuevoPaciente.PadreID}");

                    // Guardar cambios
                    var resultado = await _context.SaveChangesAsync();

                    Console.WriteLine($"Registros afectados: {resultado}");

                    // Limpiar sesión de consentimiento después de usarla
                    HttpContext.Session.Remove("ConsentimientoOtorgado");
                    HttpContext.Session.Remove("FechaConsentimiento");
                    HttpContext.Session.Remove("IPConsentimiento");

                    TempData["MensajeExito"] = $"Paciente {paciente.Nombre} {paciente.Apellido} registrado con éxito.";
                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateException dbEx)
            {
                // Error de base de datos
                TempData["MensajeError"] = "Error al guardar en la base de datos. Verifique que todos los datos sean correctos.";
                // Log del error real para debugging
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

            // Si hay errores, volver a cargar psicólogos
            ViewBag.Psicologos = new SelectList(
                _context.Usuarios.Where(u => u.RolID == 3 && u.EsActivo == true),
                "UsuarioID",
                "Nombre",
                paciente.PsicologoAsignadoID
            );

            return View(paciente);
        }

        // GET: Pacientes/Lista - Para que el padre pueda ver sus pacientes
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

        // GET: Pacientes/VerTodos - Para que el administrador pueda ver todos los pacientes
        public async Task<IActionResult> VerTodos()
        {
            // Verificar que el usuario tenga sesión válida
            var usuarioID = HttpContext.Session.GetInt32("UsuarioID");
            var rol = HttpContext.Session.GetInt32("rol");

            if (usuarioID == null)
            {
                TempData["MensajeError"] = "Sesión expirada. Por favor inicie sesión nuevamente.";
                return RedirectToAction("Login", "Login");
            }

            // Verificar que sea administrador (RolID = 1) o psicólogo (RolID = 3)
            if (rol != 1 && rol != 3)
            {
                TempData["MensajeError"] = "No tiene permisos para ver esta información.";
                return RedirectToAction("Index", "Home");
            }

            // Obtener todos los pacientes con información del padre y psicólogo
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

            // Pasar información del usuario actual a la vista
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
                // Verificar permisos del usuario
                var usuarioID = HttpContext.Session.GetInt32("UsuarioID");
                var rol = HttpContext.Session.GetInt32("rol");

                if (usuarioID == null || (rol != 1 && rol != 3))
                {
                    return Json(new { success = false, message = "No tiene permisos para realizar esta acción." });
                }

                // Buscar el paciente
                var paciente = await _context.Pacientes.FindAsync(pacienteId);
                if (paciente == null)
                {
                    return Json(new { success = false, message = "Paciente no encontrado." });
                }

                // Verificar que el psicólogo existe y está activo
                var psicologo = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.UsuarioID == psicologoId && u.RolID == 3 && u.EsActivo == true);

                if (psicologo == null)
                {
                    return Json(new { success = false, message = "Psicólogo no válido o inactivo." });
                }

                // Asignar el psicólogo
                paciente.PsicologoAsignadoID = psicologoId;

                // Guardar cambios
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

        // POST: Pacientes/RemoverPsicologo - Nuevo método para remover psicólogos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverPsicologo(int pacienteId)
        {
            try
            {
                // Verificar permisos del usuario
                var usuarioID = HttpContext.Session.GetInt32("UsuarioID");
                var rol = HttpContext.Session.GetInt32("rol");

                if (usuarioID == null || (rol != 1 && rol != 3))
                {
                    return Json(new { success = false, message = "No tiene permisos para realizar esta acción." });
                }

                // Buscar el paciente
                var paciente = await _context.Pacientes.FindAsync(pacienteId);
                if (paciente == null)
                {
                    return Json(new { success = false, message = "Paciente no encontrado." });
                }

                // Remover la asignación del psicólogo
                paciente.PsicologoAsignadoID = null;

                // Guardar cambios
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