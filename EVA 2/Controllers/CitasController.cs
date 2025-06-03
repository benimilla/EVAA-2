using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EVA_2.Data;
using EVA_2.Models;

namespace EVA_2.Controllers
{
    public class CitasController : Controller
    {
        private readonly AppDBContext _context;

        public CitasController(AppDBContext context)
        {
            _context = context;
        }

        public IActionResult Index(string fecha, string clienteId, string estado)
        {
            ViewBag.FiltroFecha = fecha ?? "";
            ViewBag.FiltroEstado = estado ?? "";

            var clientes = _context.Clientes.ToList();
            ViewBag.Clientes = new SelectList(clientes, "Id", "Nombre", clienteId);

            var estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
                new SelectListItem { Value = "Confirmada", Text = "Confirmada" },
                new SelectListItem { Value = "Cancelada", Text = "Cancelada" },
                new SelectListItem { Value = "Completado", Text = "Completado" }
            };
            ViewBag.Estados = new SelectList(estados, "Value", "Text", estado);

            var citas = _context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Servicio)
                .AsQueryable();

            if (!string.IsNullOrEmpty(fecha))
            {
                if (DateTime.TryParse(fecha, out DateTime fechaDate))
                {
                    var fechaInicio = fechaDate.Date;
                    var fechaFin = fechaDate.Date.AddDays(1);
                    citas = citas.Where(c => c.Fecha >= fechaInicio && c.Fecha < fechaFin);
                }
            }

            if (!string.IsNullOrEmpty(clienteId) && int.TryParse(clienteId, out int id))
            {
                citas = citas.Where(c => c.ClienteId == id);
            }

            if (!string.IsNullOrEmpty(estado))
            {
                citas = citas.Where(c => c.Estado == estado);
            }

            var lista = citas.ToList();
            lista = lista.OrderBy(c => c.Fecha).ThenBy(c => c.Hora).ToList();

            return View(lista);
        }

        [HttpGet]
        public IActionResult GetEventos()
        {
            var citas = _context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Servicio)
                .ToList();  // Fuerza la carga de datos antes de proyectar

            var eventos = citas.Select(c => new
            {
                id = c.Id,
                title = (c.Cliente != null ? c.Cliente.Nombre + " " + c.Cliente.Apellido : "Sin Cliente") +
                        " - " +
                        (c.Servicio != null ? c.Servicio.Nombre : "Sin Servicio"),
                start = c.Fecha.Add(c.Hora).ToString("s"), // fecha y hora en formato ISO 8601
                allDay = false
            }).ToList();

            return Json(eventos);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var cita = await _context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Servicio)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cita == null)
                return NotFound();

            return View(cita);
        }

        public IActionResult Create()
        {
            ViewBag.ClienteId = _context.Clientes
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nombre + " " + c.Apellido
                }).ToList();

            ViewBag.ServicioId = new SelectList(_context.Servicios, "Id", "Nombre");

            ViewBag.Estados = new SelectList(new List<string> { "Pendiente", "Confirmada", "Cancelada", "Completado" });

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClienteId,ServicioId,Fecha,Hora,Estado,Comentarios")] Cita cita)
        {
            // Validar solapamiento: no permitir cita para mismo servicio, fecha y hora
            bool existeSolapamiento = _context.Citas.Any(c =>
                c.Fecha == cita.Fecha &&
                c.Hora == cita.Hora &&
                c.ServicioId == cita.ServicioId);

            if (existeSolapamiento)
            {
                ModelState.AddModelError("", "Ya existe una cita para ese servicio en la fecha y hora seleccionada.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(cita);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ClienteId = _context.Clientes
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nombre + " " + c.Apellido
                }).ToList();

            ViewBag.ServicioId = new SelectList(_context.Servicios, "Id", "Nombre", cita.ServicioId);

            ViewBag.Estados = new SelectList(new List<string> { "Pendiente", "Confirmada", "Cancelada", "Completado" }, cita.Estado);

            return View(cita);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var cita = await _context.Citas.FindAsync(id);
            if (cita == null)
                return NotFound();

            ViewBag.ClienteId = _context.Clientes
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nombre + " " + c.Apellido
                }).ToList();

            ViewBag.ServicioId = new SelectList(_context.Servicios, "Id", "Nombre", cita.ServicioId);

            ViewBag.Estados = new SelectList(new List<string> { "Pendiente", "Confirmada", "Cancelada", "Completado" }, cita.Estado);

            return View(cita);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClienteId,ServicioId,Fecha,Hora,Estado,Comentarios")] Cita cita)
        {
            if (id != cita.Id)
                return NotFound();

            bool existeSolapamiento = _context.Citas.Any(c =>
                c.Id != cita.Id &&
                c.Fecha == cita.Fecha &&
                c.Hora == cita.Hora &&
                c.ServicioId == cita.ServicioId);

            if (existeSolapamiento)
            {
                ModelState.AddModelError("", "Ya existe una cita para ese servicio en la fecha y hora seleccionada.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var citaOriginal = await _context.Citas.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

                    if (citaOriginal != null && citaOriginal.Estado != cita.Estado)
                    {
                        cita.FechaCambioEstado = DateTime.Now;
                    }
                    else
                    {
                        cita.FechaCambioEstado = citaOriginal?.FechaCambioEstado;
                    }

                    _context.Update(cita);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CitaExists(cita.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // Recarga datos para la vista en caso de error
            ViewBag.ClienteId = _context.Clientes
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nombre + " " + c.Apellido
                }).ToList();

            ViewBag.ServicioId = new SelectList(_context.Servicios, "Id", "Nombre", cita.ServicioId);

            ViewBag.Estados = new SelectList(new List<string> { "Pendiente", "Confirmada", "Cancelada", "Completado" }, cita.Estado);

            return View(cita);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var cita = await _context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Servicio)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cita == null)
                return NotFound();

            return View(cita);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita != null)
                _context.Citas.Remove(cita);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CitaExists(int id)
        {
            return _context.Citas.Any(e => e.Id == id);
        }
    }
}