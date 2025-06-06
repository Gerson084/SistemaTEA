using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaTEA.Models;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Tls;
using SistemaTEA.Atributos;
using System.Security.Cryptography;

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

        // GET: Registro
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

                usuario.RolID = 2;
                usuario.Contraseña = SeguridadHelper.HashPassword(usuario.Contraseña);
                usuario.EsActivo = false; // pendiente de aprobación

                try
                {
                    _testContext.Usuarios.Add(usuario);
                    _testContext.SaveChanges();

                    HttpContext.Session.SetInt32("id_usuario", usuario.UsuarioID);
                    HttpContext.Session.SetString("nombre", usuario.Nombre);
                    HttpContext.Session.SetInt32("rol", usuario.RolID);
                    HttpContext.Session.SetString("telefono", usuario.Telefono ?? "");

                    TempData["RegistroExitoso"] = "Registro exitoso. En un periodo de 24 horas un agente se comunicará contigo para brindarte detalles del estado de tu cuenta.";

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

        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public IActionResult Login(string correo, string clave)
        {
            var user = _testContext.Usuarios.FirstOrDefault(u => u.Email == correo);

            if (user != null && SeguridadHelper.VerificarPassword(clave, user.Contraseña))
            {
                if (!user.EsActivo)
                {
                    ViewBag.Error = "Tu cuenta aún no ha sido aprobada por un administrador.";
                    return View();
                }

                // Establecer variables de sesión
                HttpContext.Session.SetInt32("id_usuario", user.UsuarioID);
                HttpContext.Session.SetString("nombre", user.Nombre);
                HttpContext.Session.SetInt32("rol", user.RolID);

                // Redirección según rol del usuario
                return user.RolID switch
                {
                    1 => RedirectToAction("InicioADMIN", "Inicio"), // Administrador
                    2 => RedirectToAction("InicioPADRE", "Inicio"), // Padre
                    3 => RedirectToAction("InicioPSICOLOGO", "Inicio"),// Psicólogo
                      => RedirectToAction("Index", "Home")
                };
            }

            ViewBag.Error = "Correo o clave incorrectos.";
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }

}
