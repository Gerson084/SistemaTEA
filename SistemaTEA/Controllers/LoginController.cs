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

        [HttpPost]
        public IActionResult Registro(Usuario usuario, string confirmarContrasena)
        {
            usuario.RolID = 2;
            if (ModelState.IsValid)
            {
                if (usuario.Contraseña != confirmarContrasena)
                {
                    ViewBag.Error = "Las contraseñas no coinciden.";
                    return View(usuario);
                }

                var existe = _testContext.Usuarios.FirstOrDefault(u => u.Email == usuario.Email);
                if (existe != null)
                {
                    ViewBag.Error = "El correo ya está registrado.";
                    return View(usuario);
                }

                var ContraseñaSinHash = usuario.Contraseña;

                usuario.RolID = 2;
                usuario.Contraseña = SeguridadHelper.HashPassword(usuario.Contraseña);
                usuario.EsActivo = false; // pendiente de aprobación

                try
                {
                    _testContext.Usuarios.Add(usuario);
                    _testContext.SaveChanges();

                    HttpContext.Session.SetInt32("UsuarioID", usuario.UsuarioID); // CAMBIO AQUI
                    HttpContext.Session.SetString("nombre", usuario.Nombre);
                    HttpContext.Session.SetInt32("rol", usuario.RolID);
                    HttpContext.Session.SetString("telefono", usuario.Telefono ?? "");

                    TempData["RegistroExitoso"] = "Registro exitoso. En un periodo de 24 horas un agente se comunicará contigo para brindarte detalles del estado de tu cuenta.";

                    var emailService = new EmailService();
                    emailService.EnviarCorreoBienvenida(usuario.Email, ContraseñaSinHash);

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
                new Claim(ClaimTypes.Role, user.RolID == 1 ? "Administrador" :
                                             user.RolID == 2 ? "Padre" : "Psicologo"),
                new Claim(ClaimTypes.NameIdentifier, user.UsuarioID.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            HttpContext.Session.SetInt32("UsuarioID", user.UsuarioID); // CAMBIO AQUI
            HttpContext.Session.SetString("nombre", user.Nombre);
            HttpContext.Session.SetInt32("rol", user.RolID);
            HttpContext.Session.SetString("telefono", user.Telefono ?? "");

            return user.RolID switch
            {
                1 => RedirectToAction("InicioADMIN", "Login"),
                2 => RedirectToAction("InicioPADRE", "Login"),
                3 => RedirectToAction("InicioPSICOLOGO", "Login"),
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
    }
}
