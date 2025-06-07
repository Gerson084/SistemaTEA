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
                usuario.EsActivo = true;
                usuario.FechaAprobacion = DateTime.Now;
                usuario.AprobadoPor = idAdmin.Value;

                await _context.SaveChangesAsync();

                try
                {

                    var emailService = new EmailService();
                    emailService.EnviarCorreoConfirmacionAcceso(usuario.Email);
                }
                catch (Exception emailError)
                {
                    Console.WriteLine("Error al enviar correo: " + emailError.Message);

                }

                return Json(new { success = true, message = "Cuenta activada y notificación enviada al correo del padre." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al activar la cuenta: " + ex.Message });
            }
        }


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

        [HttpGet]
        public IActionResult RegistrarPsicologo()
        {
            if (!SesionValida(out int? rol)) return RedirectToAction("Login", "Login");
            if (rol != 1) return RedirectToAction("Login", "Login");

            ViewBag.NombreUsuario = HttpContext.Session.GetString("nombre") ?? "Invitado";
            ViewData["rol"] = rol;

            return View();
        }

        // POST: Registrar nuevo psicólogo (CUENTA ACTIVA INMEDIATAMENTE)
        [HttpPost]
        public async Task<IActionResult> RegistrarPsicologo(Usuario psicologo, string confirmarContrasena)
        {
            if (!SesionValida(out int? rol, out int? idAdmin))
                return Json(new { success = false, message = "Sesión no válida." });

            if (rol != 1 || idAdmin == null)
                return Json(new { success = false, message = "No tiene permisos para esta acción." });

            // Establecer el rol como psicólogo
            psicologo.RolID = 3;

            if (ModelState.IsValid)
            {
                if (psicologo.Contraseña != confirmarContrasena)
                {
                    return Json(new { success = false, message = "Las contraseñas no coinciden." });
                }

                var existe = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == psicologo.Email);
                if (existe != null)
                {
                    return Json(new { success = false, message = "El correo ya está registrado." });
                }

                try
                {
                    var contraseñaSinHash = psicologo.Contraseña;

                    psicologo.Contraseña = SeguridadHelper.HashPassword(psicologo.Contraseña);
                    psicologo.EsActivo = true;
                    psicologo.FechaRegistro = DateTime.Now;
                    psicologo.FechaAprobacion = DateTime.Now;
                    psicologo.AprobadoPor = idAdmin.Value;

                    _context.Usuarios.Add(psicologo);
                    await _context.SaveChangesAsync();

                    try
                    {
                        var emailService = new EmailService();
                        emailService.EnviarCorreoBienvenidaPsicologo(psicologo.Email, contraseñaSinHash);
                    }
                    catch (Exception emailError)
                    {
                        Console.WriteLine("Error al enviar correo: " + emailError.Message);
                    }

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error al registrar el psicólogo: " + ex.Message });
                }
            }


            return Json(new { success = false, message = "Datos inválidos. Verifique el formulario." });
        }



        public async Task<IActionResult> VerPsicologos()
        {
            if (!SesionValida(out int? rol)) return RedirectToAction("Login", "Login");
            if (rol != 1) return RedirectToAction("Login", "Login");

            ViewBag.NombreUsuario = HttpContext.Session.GetString("nombre") ?? "Invitado";
            ViewData["rol"] = rol;

            var psicologos = await _context.Usuarios
                .Where(u => u.RolID == 3)
                .OrderBy(u => u.Nombre)
                .ToListAsync();

            return View(psicologos);
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerDetallesPsicologo(int usuarioId)
        {
            if (!SesionValida(out int? rol))
                return Json(new { success = false, message = "Sesión no válida." });

            if (rol != 1)
                return Json(new { success = false, message = "No tiene permisos." });

            var psicologo = await _context.Usuarios
                .Where(u => u.UsuarioID == usuarioId && u.RolID == 3)
                .Select(u => new
                {
                    u.UsuarioID,
                    u.Nombre,
                    u.Apellido,
                    u.Email,
                    u.Telefono,
                    u.FechaRegistro,
                    u.FechaAprobacion,
                    u.EsActivo
                })
                .FirstOrDefaultAsync();

            if (psicologo == null)
                return Json(new { success = false, message = "Psicólogo no encontrado." });

            return Json(new { success = true, psicologo });
        }


        [HttpPost]
        public async Task<IActionResult> CambiarEstadoPsicologo(int usuarioId, bool activar)
        {
            if (!SesionValida(out int? rol, out int? idAdmin))
                return Json(new { success = false, message = "Sesión no válida." });

            if (rol != 1 || idAdmin == null)
                return Json(new { success = false, message = "No tiene permisos para esta acción." });

            var psicologo = await _context.Usuarios.FindAsync(usuarioId);
            if (psicologo == null)
                return Json(new { success = false, message = "Psicólogo no encontrado." });

            if (psicologo.RolID != 3)
                return Json(new { success = false, message = "El usuario no es un psicólogo." });

            try
            {
                psicologo.EsActivo = activar;

                if (activar && psicologo.FechaAprobacion == null)
                {
                    psicologo.FechaAprobacion = DateTime.Now;
                    psicologo.AprobadoPor = idAdmin.Value;
                }

                await _context.SaveChangesAsync();

                string mensaje = activar ? "Psicólogo activado exitosamente." : "Psicólogo desactivado exitosamente.";
                return Json(new { success = true, message = mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al cambiar el estado: " + ex.Message });
            }
        }


        private bool SesionValida(out int? rol)
        {
            rol = HttpContext.Session.GetInt32("rol");
            return HttpContext.Session.GetInt32("UsuarioID") != null;
        }

        private bool SesionValida(out int? rol, out int? idUsuario)
        {
            rol = HttpContext.Session.GetInt32("rol");
            idUsuario = HttpContext.Session.GetInt32("UsuarioID");
            return idUsuario != null;
        }


        public async Task<IActionResult> VerUsuarios()
        {
            if (!SesionValida(out int? rol)) return RedirectToAction("Login", "Login");
            if (rol != 1) return RedirectToAction("Login", "Login");

            ViewBag.NombreUsuario = HttpContext.Session.GetString("nombre") ?? "Invitado";
            ViewData["rol"] = rol;

            var padresActivos = await _context.Usuarios
                .Where(u => u.RolID == 2 && u.EsActivo)
                .OrderBy(u => u.Nombre)
                .ToListAsync();

            return View(padresActivos);
        }




    }
}