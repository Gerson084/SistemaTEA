using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTEA.Models;
using SistemaTEA.Atributos;

namespace SistemaTEA.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly EvaluacionContext _context;

        public UsuariosController(EvaluacionContext context)
        {
            _context = context;
        }

        // GET: Usuarios (Solo para administradores - muestra padres pendientes de activación)
        public async Task<IActionResult> Index()
        {
            if (!SesionValida(out int? rol)) return RedirectToAction("Login", "Login");
            if (rol != 1) return RedirectToAction("Login", "Login");

            ViewBag.NombreUsuario = HttpContext.Session.GetString("nombre") ?? "Invitado";
            ViewData["rol"] = rol;

            var padresPendientes = await _context.Usuarios
                .Where(u => u.RolID == 2 && !u.EsActivo)
                .OrderBy(u => u.FechaRegistro)
                .ToListAsync();

            return View(padresPendientes);
        }

        // POST: Activar cuenta de padre
        [HttpPost]
        public async Task<IActionResult> ActivarCuenta(int usuarioId)
        {
            if (!SesionValida(out int? rol, out int? idAdmin))
                return Json(new { success = false, message = "Sesión no válida." });

            if (rol != 1 || idAdmin == null)
                return Json(new { success = false, message = "No tiene permisos para esta acción." });

            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
                return Json(new { success = false, message = "Usuario no encontrado." });

            if (usuario.RolID != 2)
                return Json(new { success = false, message = "Solo se pueden activar cuentas de padres." });

            if (usuario.EsActivo)
                return Json(new { success = false, message = "La cuenta ya está activa." });

            try
            {
                // Activar cuenta
                usuario.EsActivo = true;
                usuario.FechaAprobacion = DateTime.Now;
                usuario.AprobadoPor = idAdmin.Value;

                await _context.SaveChangesAsync();

                try
                {
                    // Enviar correo de confirmación
                    var emailService = new EmailService();
                    emailService.EnviarCorreoConfirmacionAcceso(usuario.Email);
                }
                catch (Exception emailError)
                {
                    Console.WriteLine("Error al enviar correo: " + emailError.Message);
                    // Continúa aunque falle el envío de correo
                }

                return Json(new { success = true, message = "Cuenta activada y notificación enviada al correo del padre." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al activar la cuenta: " + ex.Message });
            }
        }

        // GET: Obtener detalles de un padre (solo para administradores)
        [HttpGet]
        public async Task<IActionResult> ObtenerDetallesUsuario(int usuarioId)
        {
            if (!SesionValida(out int? rol))
                return Json(new { success = false, message = "Sesión no válida." });

            if (rol != 1)
                return Json(new { success = false, message = "No tiene permisos." });

            var usuario = await _context.Usuarios
                .Where(u => u.UsuarioID == usuarioId && u.RolID == 2)
                .Select(u => new
                {
                    u.UsuarioID,
                    u.Nombre,
                    u.Apellido,
                    u.Email,
                    u.Telefono,
                    u.FechaRegistro,
                    u.EsActivo
                })
                .FirstOrDefaultAsync();

            if (usuario == null)
                return Json(new { success = false, message = "Usuario no encontrado." });

            return Json(new { success = true, usuario });
        }

        // --------------------
        // Métodos de validación
        // --------------------

        private bool SesionValida(out int? rol)
        {
            rol = HttpContext.Session.GetInt32("rol");
            return HttpContext.Session.GetInt32("id_usuario") != null;
        }

        private bool SesionValida(out int? rol, out int? idUsuario)
        {
            rol = HttpContext.Session.GetInt32("rol");
            idUsuario = HttpContext.Session.GetInt32("id_usuario");
            return idUsuario != null;
        }
    }
}
