using Microsoft.AspNetCore.Mvc;

namespace SistemaTEA.Controllers
{
    public class UsuariosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
