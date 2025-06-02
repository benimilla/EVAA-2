using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EVA_2.Data;
using EVA_2.Models;

namespace EVA_2.Controllers
{
    public class ServiciosController : Controller
    {
        private readonly AppDBContext _context;

        public ServiciosController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Servicios
        public async Task<IActionResult> Index(string estado)
        {
            var servicios = from s in _context.Servicios select s;

            if (estado == "activos")
            {
                servicios = servicios.Where(s => s.Activo);
            }
            else if (estado == "inactivos")
            {
                servicios = servicios.Where(s => !s.Activo);
            }

            ViewData["EstadoActual"] = estado;
            return View(await servicios.ToListAsync());
        }

        // GET: Servicios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var servicio = await _context.Servicios.FirstOrDefaultAsync(m => m.Id == id);
            if (servicio == null) return NotFound();

            return View(servicio);
        }

        // GET: Servicios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Servicios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Duracion,Descripcion,Precio,Activo")] Servicio servicio)
        {
            // Validaciones manuales extra (además de DataAnnotations en el modelo)
            if (string.IsNullOrWhiteSpace(servicio.Nombre))
                ModelState.AddModelError("Nombre", "El nombre es obligatorio.");

            if (servicio.Precio <= 0)
                ModelState.AddModelError("Precio", "El precio debe ser un valor positivo.");

            if (servicio.Duracion <= 0)
                ModelState.AddModelError("Duracion", "La duración debe ser un valor positivo.");

            if (ModelState.IsValid)
            {
                servicio.Activo = true; // Por defecto activo al crear
                _context.Add(servicio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(servicio);
        }

        // GET: Servicios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null) return NotFound();

            return View(servicio);
        }

        // POST: Servicios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Duracion,Descripcion,Precio,Activo")] Servicio servicio)
        {
            if (id != servicio.Id) return NotFound();

            if (string.IsNullOrWhiteSpace(servicio.Nombre))
                ModelState.AddModelError("Nombre", "El nombre es obligatorio.");

            if (servicio.Precio <= 0)
                ModelState.AddModelError("Precio", "El precio debe ser un valor positivo.");

            if (servicio.Duracion <= 0)
                ModelState.AddModelError("Duracion", "La duración debe ser un valor positivo.");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(servicio);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServicioExists(servicio.Id))
                        return NotFound();
                    else throw;
                }
            }
            return View(servicio);
        }

        // GET: Servicios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var servicio = await _context.Servicios.FirstOrDefaultAsync(m => m.Id == id);
            if (servicio == null) return NotFound();

            return View(servicio);
        }

        // POST: Servicios/Delete/5 (Eliminación lógica)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null) return NotFound();

            // Validar si tiene citas pendientes asociadas
            bool tieneCitasPendientes = await _context.Citas
                .AnyAsync(c => c.ServicioId == id && c.Estado == "Pendiente");

            if (tieneCitasPendientes)
            {
                TempData["Error"] = "No se puede eliminar el servicio porque tiene citas pendientes asociadas.";
                return RedirectToAction(nameof(Index));
            }

            // Eliminación lógica: cambiar activo a false
            servicio.Activo = false;
            _context.Update(servicio);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ServicioExists(int id)
        {
            return _context.Servicios.Any(e => e.Id == id);
        }
    }
}