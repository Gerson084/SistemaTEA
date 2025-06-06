using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaTEA.Models;

namespace SistemaTEA.Controllers
{
    public class PreguntaMCHATsController : Controller
    {
        private readonly EvaluacionContext _context;

        public PreguntaMCHATsController(EvaluacionContext context)
        {
            _context = context;
        }

        // GET: PreguntaMCHATs
        public async Task<IActionResult> Index()
        {
            return View(await _context.PreguntasMCHAT.ToListAsync());
        }

        // GET: PreguntaMCHATs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var preguntaMCHAT = await _context.PreguntasMCHAT
                .FirstOrDefaultAsync(m => m.PreguntaID == id);
            if (preguntaMCHAT == null)
            {
                return NotFound();
            }

            return View(preguntaMCHAT);
        }

        // GET: PreguntaMCHATs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PreguntaMCHATs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PreguntaID,NumeroPregunta,TextoPregunta,EsActiva")] PreguntaMCHAT preguntaMCHAT)
        {
            if (ModelState.IsValid)
            {
                _context.Add(preguntaMCHAT);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(preguntaMCHAT);
        }

        // GET: PreguntaMCHATs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var preguntaMCHAT = await _context.PreguntasMCHAT.FindAsync(id);
            if (preguntaMCHAT == null)
            {
                return NotFound();
            }
            return View(preguntaMCHAT);
        }

        // POST: PreguntaMCHATs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PreguntaID,NumeroPregunta,TextoPregunta,EsActiva")] PreguntaMCHAT preguntaMCHAT)
        {
            if (id != preguntaMCHAT.PreguntaID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(preguntaMCHAT);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PreguntaMCHATExists(preguntaMCHAT.PreguntaID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(preguntaMCHAT);
        }

        // GET: PreguntaMCHATs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var preguntaMCHAT = await _context.PreguntasMCHAT
                .FirstOrDefaultAsync(m => m.PreguntaID == id);
            if (preguntaMCHAT == null)
            {
                return NotFound();
            }

            return View(preguntaMCHAT);
        }

        // POST: PreguntaMCHATs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var preguntaMCHAT = await _context.PreguntasMCHAT.FindAsync(id);
            if (preguntaMCHAT != null)
            {
                _context.PreguntasMCHAT.Remove(preguntaMCHAT);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PreguntaMCHATExists(int id)
        {
            return _context.PreguntasMCHAT.Any(e => e.PreguntaID == id);
        }
    }
}
