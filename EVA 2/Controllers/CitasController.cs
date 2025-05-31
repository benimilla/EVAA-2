using EVA_2.Data;
using EVA_2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EVA_2.Controllers
{
    public class CitasController : Controller
    {
        private readonly AppDBContext _context;

        public CitasController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Citas
        public async Task<IActionResult> Index()
        {
            var citas = _context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Servicio)
                .Include(c => c.Estado); // <- Asegura incluir Estado
            return View(await citas.ToListAsync());
        }

        // GET: Citas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var cita = await _context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Servicio)
                .Include(c => c.Estado)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cita == null) return NotFound();

            return View(cita);
        }

        // GET: Citas/Create
        public IActionResult Create()
        {
            ViewBag.Clientes = new SelectList(_context.Clientes, "Id", "Nombre");
            ViewBag.Servicios = new SelectList(_context.Servicios, "Id", "Nombre");
            ViewBag.Estados = new SelectList(_context.Estados, "Id", "Nombre");
            return View();
        }

        // POST: Citas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Cita cita, string nuevoEstadoNombre)
        {
            if (!string.IsNullOrWhiteSpace(nuevoEstadoNombre))
            {
                var estadoExistente = _context.Estados.FirstOrDefault(e => e.Nombre.ToLower() == nuevoEstadoNombre.ToLower());
                if (estadoExistente != null)
                {
                    cita.EstadoId = estadoExistente.Id;
                }
                else
                {
                    var nuevoEstado = new Estado { Nombre = nuevoEstadoNombre };
                    _context.Estados.Add(nuevoEstado);
                    _context.SaveChanges();
                    cita.EstadoId = nuevoEstado.Id;
                }
            }

            if (ModelState.IsValid)
            {
                _context.Citas.Add(cita);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Clientes = new SelectList(_context.Clientes, "Id", "Nombre", cita.ClienteId);
            ViewBag.Servicios = new SelectList(_context.Servicios, "Id", "Nombre", cita.ServicioId);
            ViewBag.Estados = new SelectList(_context.Estados, "Id", "Nombre", cita.EstadoId);
            return View(cita);
        }

        // GET: Citas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cita = await _context.Citas.FindAsync(id);
            if (cita == null) return NotFound();

            ViewBag.Clientes = new SelectList(_context.Clientes, "Id", "Nombre", cita.ClienteId);
            ViewBag.Servicios = new SelectList(_context.Servicios, "Id", "Nombre", cita.ServicioId);
            ViewBag.Estados = new SelectList(_context.Estados, "Id", "Nombre", cita.EstadoId);

            return View(cita);
        }

        // POST: Citas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Cita cita, string nuevoEstadoNombre)
        {
            if (id != cita.Id) return NotFound();

            if (!string.IsNullOrWhiteSpace(nuevoEstadoNombre))
            {
                var estadoExistente = _context.Estados.FirstOrDefault(e => e.Nombre.ToLower() == nuevoEstadoNombre.ToLower());
                if (estadoExistente != null)
                {
                    cita.EstadoId = estadoExistente.Id;
                }
                else
                {
                    var nuevoEstado = new Estado { Nombre = nuevoEstadoNombre };
                    _context.Estados.Add(nuevoEstado);
                    _context.SaveChanges();
                    cita.EstadoId = nuevoEstado.Id;
                }
            }

            if (ModelState.IsValid)
            {
                _context.Update(cita);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Clientes = new SelectList(_context.Clientes, "Id", "Nombre", cita.ClienteId);
            ViewBag.Servicios = new SelectList(_context.Servicios, "Id", "Nombre", cita.ServicioId);
            ViewBag.Estados = new SelectList(_context.Estados, "Id", "Nombre", cita.EstadoId);
            return View(cita);
        }

        // GET: Citas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var cita = await _context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Servicio)
                .Include(c => c.Estado)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cita == null) return NotFound();

            return View(cita);
        }

        // POST: Citas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita != null) _context.Citas.Remove(cita);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CitaExists(int id)
        {
            return _context.Citas.Any(e => e.Id == id);
        }
    }
}
