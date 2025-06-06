using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTEA.Models;

namespace SistemaTEA.Controllers
{
    public class UsuariosController : Controller
    {

        private readonly EvaluacionContext _context;

        public UsuariosController(EvaluacionContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUsuario([Bind("UsuarioID,Nombre,Correo,Contrasena")] Usuario usuario)
        {
            // Verificar si el correo ya existe
            var usuarioExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == usuario.Email);

            if (usuarioExistente != null)
            {
                TempData["ErrorMessage"] = "El correo ya está registrado.";
                return RedirectToAction("Login", "Login");
            }

            // Validar la contraseña manualmente en el backend
            if (!System.Text.RegularExpressions.Regex.IsMatch(usuario.Contraseña, @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"))
            {
                TempData["ErrorMessage"] = "La contraseña debe tener al menos 8 caracteres, incluir una letra, un número y un carácter especial.";
                return RedirectToAction("Login", "Login");
            }




            // Asignar el rol predeterminado "Usuario"
            usuario.RolID = 2;

            // Si pasa todas las validaciones, guardar en la BD
            if (ModelState.IsValid)
            {
                var passwordHasher = new PasswordHasher<Usuario>();
                usuario.Contraseña = passwordHasher.HashPassword(usuario, usuario.Contraseña);

                _context.Add(usuario);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Usuario registrado correctamente. Se le notificará cuando tenga acceso al sistema";
                return RedirectToAction("Login", "Login");
            }

            return RedirectToAction("Login", "Login");
        }
    }
}
