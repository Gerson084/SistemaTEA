using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaTEA.Models;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SistemaTEA.Atributos;

namespace SistemaTEA.Controllers
{
    public class LoginController : Controller
    {
        private readonly EvaluacionContext _testContext;

        public LoginController(EvaluacionContext context)
        {
            _testContext = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Registro()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult Registro(Usuario usuario, string confirmarContrasena, bool esPadre = false)
        //{

        //    usuario.RolID = esPadre ? 2 : 4; 

        //    if (ModelState.IsValid)
        //    {
        //        var regex = new Regex(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$");


        //        if (usuario.Contraseña != confirmarContrasena)
        //        {
        //            ViewBag.Error = "Las contraseñas no coinciden.";
        //            return View(usuario);
        //        }


        //        if (!regex.IsMatch(usuario.Contraseña))
        //        {
        //            TempData["Error_Contrasena"] = "La contraseña debe tener al menos una letra mayúscula, un número y un carácter especial.";
        //            ViewBag.Error = "La contraseña no cumple con los requisitos de seguridad.";
        //            return View(usuario);
        //        }

        //        var existe = _testContext.Usuarios.FirstOrDefault(u => u.Email == usuario.Email);
        //        if (existe != null)
        //        {
        //            ViewBag.Error = "El correo ya está registrado.";
        //            return View(usuario);
        //        }

        //        var ContraseñaSinHash = usuario.Contraseña;


        //        usuario.Contraseña = SeguridadHelper.HashPassword(usuario.Contraseña);
        //        usuario.EsActivo = false;

        //        try
        //        {
        //            _testContext.Usuarios.Add(usuario);
        //            _testContext.SaveChanges();

        //            HttpContext.Session.SetInt32("UsuarioID", usuario.UsuarioID);
        //            HttpContext.Session.SetString("nombre", usuario.Nombre);
        //            HttpContext.Session.SetInt32("rol", usuario.RolID);
        //            HttpContext.Session.SetString("telefono", usuario.Telefono ?? "");


        //            string mensajeExito = esPadre
        //                ? "Registro exitoso como padre/madre. En un periodo de 24 horas un agente se comunicará contigo para brindarte detalles del estado de tu cuenta y el proceso de evaluación para tu hijo/a."
        //                : "Registro exitoso. En un periodo de 24 horas un agente se comunicará contigo para brindarte detalles del estado de tu cuenta.";

        //            TempData["RegistroExitoso"] = mensajeExito;


        //            var emailService = new EmailService();
        //            emailService.EnviarCorreoBienvenida(usuario.Email, ContraseñaSinHash);

        //            return RedirectToAction("Login", "Login");
        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.Error = "Ocurrió un error al registrar el usuario: " + ex.Message;
        //            return View(usuario);
        //        }
        //    }

        //    return View(usuario);
        //}

        [HttpPost]
        public IActionResult Registro(Usuario usuario, string confirmarContrasena, DateTime? fechaNacimiento, string genero, bool esPadre = false)
        {
            // Asignar rol: si es padre = 2, si no es padre (paciente) = 4
            usuario.RolID = esPadre ? 2 : 4;

            if (ModelState.IsValid)
            {
                var regex = new Regex(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$");

                // Validaciones
                if (usuario.Contraseña != confirmarContrasena)
                {
                    ViewBag.Error = "Las contraseñas no coinciden.";
                    return View(usuario);
                }

                if (!regex.IsMatch(usuario.Contraseña))
                {
                    ViewBag.Error = "La contraseña debe tener al menos una letra mayúscula, un número y un carácter especial.";
                    return View(usuario);
                }

                // Validar campos adicionales si NO es padre (es paciente)
                if (!esPadre)
                {
                    if (!fechaNacimiento.HasValue)
                    {
                        ViewBag.Error = "La fecha de nacimiento es requerida para pacientes.";
                        return View(usuario);
                    }

                    if (string.IsNullOrEmpty(genero) || (genero != "M" && genero != "F"))
                    {
                        ViewBag.Error = "Debe seleccionar un género válido (M o F).";
                        return View(usuario);
                    }
                }

                var existe = _testContext.Usuarios.FirstOrDefault(u => u.Email == usuario.Email);
                if (existe != null)
                {
                    ViewBag.Error = "El correo ya está registrado.";
                    return View(usuario);
                }

                var contraseñaSinHash = usuario.Contraseña;
                usuario.Contraseña = SeguridadHelper.HashPassword(usuario.Contraseña);
                usuario.EsActivo = false;

                try
                {
                    // Insertar usuario
                    _testContext.Usuarios.Add(usuario);
                    _testContext.SaveChanges();

                    // Si NO es padre, también crear registro en tabla Pacientes
                    if (!esPadre)
                    {
                        var paciente = new Paciente
                        {
                            Nombre = usuario.Nombre,
                            Apellido = usuario.Apellido,
                            FechaNacimiento = fechaNacimiento.Value,
                            Genero = genero,
                            PadreID = usuario.UsuarioID,
                            PsicologoAsignadoID  = usuario.UsuarioID,
                        };

                        _testContext.Pacientes.Add(paciente);
                        _testContext.SaveChanges();
                    }

                    // Session
                    HttpContext.Session.SetInt32("UsuarioID", usuario.UsuarioID);
                    HttpContext.Session.SetString("nombre", usuario.Nombre);
                    HttpContext.Session.SetInt32("rol", usuario.RolID);
                    HttpContext.Session.SetString("telefono", usuario.Telefono ?? "");
                    HttpContext.Session.SetInt32("PacienteID", usuario.UsuarioID);
                    string mensajeExito = esPadre
                        ? "Registro exitoso como padre/madre. En 24 horas un agente se comunicará contigo."
                        : "Registro exitoso como paciente. En 24 horas un agente se comunicará contigo.";

                    TempData["RegistroExitoso"] = mensajeExito;

                    var emailService = new EmailService();
                    emailService.EnviarCorreoBienvenida(usuario.Email, contraseñaSinHash);

                    return RedirectToAction("Login", "Login");
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Ocurrió un error al registrar el usuario: " + ex.Message;
                    return View(usuario);
                }
            }

            return View(usuario);
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string correo, string clave)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(clave))
            {
                ViewBag.Error = "Debe completar todos los campos.";
                return View();
            }

            string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(correo, emailRegex))
            {
                ViewBag.Error = "El correo electrónico no es válido.";
                return View();
            }

            var user = await _testContext.Usuarios.FirstOrDefaultAsync(u => u.Email == correo);
            if (user == null)
            {
                ViewBag.Error = "Usuario no encontrado.";
                return View();
            }

            if (!SeguridadHelper.VerificarPassword(clave, user.Contraseña))
            {
                ViewBag.Error = "Correo o clave incorrectos.";
                return View();
            }

            if (!user.EsActivo)
            {
                ViewBag.Error = "Tu cuenta aún no ha sido aprobada por un administrador.";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Nombre),
                new Claim(ClaimTypes.Role,
                    user.RolID == 1 ? "Administrador" :
                    user.RolID == 2 ? "Padre" :
                    user.RolID == 3 ? "Psicologo" :
                    "Paciente"), 
                new Claim(ClaimTypes.NameIdentifier, user.UsuarioID.ToString())
            };


            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            HttpContext.Session.SetInt32("UsuarioID", user.UsuarioID); 
            HttpContext.Session.SetString("nombre", user.Nombre);
            HttpContext.Session.SetInt32("rol", user.RolID);
            HttpContext.Session.SetString("telefono", user.Telefono ?? "");

            return user.RolID switch
            {
                1 => RedirectToAction("InicioADMIN", "Login"),
                2 => RedirectToAction("InicioPADRE", "Login"),
                3 => RedirectToAction("InicioPSICOLOGO", "Login"),
                4 => RedirectToAction("InicioPACIENTE", "Login"),
                _ => RedirectToAction("Login", "Login")
            };
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Login");
        }

        // ✅ CAMBIO REALIZADO: Se usa "UsuarioID"
        private bool ValidarSesion()
        {
            return HttpContext.Session.GetInt32("UsuarioID") != null;
        }

        [HttpGet]
        public ActionResult InicioADMIN()
        {
            if (!ValidarSesion()) return RedirectToAction("Login", "Login");
            ViewBag.NombreUsuario = HttpContext.Session.GetString("nombre") ?? "Invitado";
            ViewData["rol"] = HttpContext.Session.GetInt32("rol") ?? 0;
            return View();
        }

        [HttpGet]
        public ActionResult InicioPADRE()
        {
            if (!ValidarSesion()) return RedirectToAction("Login", "Login");
            ViewBag.NombreUsuario = HttpContext.Session.GetString("nombre") ?? "Invitado";
            ViewData["rol"] = HttpContext.Session.GetInt32("rol") ?? 0;
            return View();
        }

        [HttpGet]
        public ActionResult InicioPSICOLOGO()
        {
            if (!ValidarSesion()) return RedirectToAction("Login", "Login");
            ViewBag.NombreUsuario = HttpContext.Session.GetString("nombre") ?? "Invitado";
            ViewData["rol"] = HttpContext.Session.GetInt32("rol") ?? 0;
            return View();
        }

        [HttpGet]
        public ActionResult InicioPACIENTE()
        {
            if (!ValidarSesion()) return RedirectToAction("Login", "Login");
            ViewBag.NombreUsuario = HttpContext.Session.GetString("nombre") ?? "Invitado";
            ViewData["rol"] = HttpContext.Session.GetInt32("rol") ?? 0;
            return View();
        }
    }
}
