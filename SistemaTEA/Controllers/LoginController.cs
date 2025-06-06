using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaTEA.Models;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace SistemaTEA.Controllers
{
    public class LoginController : Controller
    {
        private readonly EvaluacionContext _context;

        public LoginController(EvaluacionContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // POST: Procesa el inicio de sesión
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string correo, string contrasena)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena))
            {
                ModelState.AddModelError("", "Debe completar todos los campos.");
                return View();
            }

            string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(correo, emailRegex))
            {
                ModelState.AddModelError("", "El correo electrónico no es válido.");
                return View();
            }

            // Buscar al usuario
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == correo);

            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuario no encontrado.");
                return View();
            }

            // Verificar la contraseña
            var passwordHasher = new PasswordHasher<Usuario>();
            var resultado = passwordHasher.VerifyHashedPassword(usuario, usuario.Contraseña, contrasena);

            if (resultado == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Correo o contraseña incorrectos.");
                return View();
            }

            if (usuario.RolID == null)
            {
                ModelState.AddModelError("", "El usuario no tiene un rol asignado.");
                return View();
            }

            // 🔐 Claims para autenticación
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Role, usuario.RolID == 1 ? "Administrador" :
                                   usuario.RolID == 2 ? "Padre" : "Psicologo"),
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioID.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // También puedes mantener la sesión si deseas usarla en otras vistas
            HttpContext.Session.SetInt32("usuarioId", usuario.UsuarioID);
            HttpContext.Session.SetString("nombreUsuario", usuario.Nombre);
            HttpContext.Session.SetInt32("TipoUsuario", usuario.RolID);

            // Redirección según rol
            return usuario.RolID switch
            {
                1 => RedirectToAction("Index", "Home"),
                2 => RedirectToAction("Index", "Home"),
                3 => RedirectToAction("Index", "Home"),
                _ => RedirectToAction("Index", "Home")
            };
        }
    }
}
