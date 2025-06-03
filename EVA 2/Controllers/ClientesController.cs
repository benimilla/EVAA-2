using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EVA_2.Data;
using EVA_2.Models;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace EVA_2.Controllers
{
    public class ClientesController : Controller
    {
        private readonly AppDBContext _context;
        private const int pageSize = 10;

        public ClientesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index(string sortOrder, int page = 1)
        {
            // Verifica si la tabla está vacía
            if (!_context.Clientes.Any())
            {
                var clientes = new Cliente[]
                {
            new Cliente { Nombre = "Carlos", Apellido = "García", Email = "carlos@example.com", Telefono = "1234567890", FechaRegistro = DateTime.Now.AddDays(-10) },
            new Cliente { Nombre = "Ana", Apellido = "Martínez", Email = "ana@example.com", Telefono = "1234567891", FechaRegistro = DateTime.Now.AddDays(-9) },
            new Cliente { Nombre = "Juan", Apellido = "Pérez", Email = "juan@example.com", Telefono = "1234567892", FechaRegistro = DateTime.Now.AddDays(-8) },
            new Cliente { Nombre = "Laura", Apellido = "Sánchez", Email = "laura@example.com", Telefono = "1234567893", FechaRegistro = DateTime.Now.AddDays(-7) },
            new Cliente { Nombre = "Miguel", Apellido = "Torres", Email = "miguel@example.com", Telefono = "1234567894", FechaRegistro = DateTime.Now.AddDays(-6) },
            new Cliente { Nombre = "María", Apellido = "López", Email = "maria@example.com", Telefono = "1234567895", FechaRegistro = DateTime.Now.AddDays(-5) },
            new Cliente { Nombre = "José", Apellido = "Hernández", Email = "jose@example.com", Telefono = "1234567896", FechaRegistro = DateTime.Now.AddDays(-4) },
            new Cliente { Nombre = "Elena", Apellido = "Díaz", Email = "elena@example.com", Telefono = "1234567897", FechaRegistro = DateTime.Now.AddDays(-3) },
            new Cliente { Nombre = "Pedro", Apellido = "Ruiz", Email = "pedro@example.com", Telefono = "1234567898", FechaRegistro = DateTime.Now.AddDays(-2) },
            new Cliente { Nombre = "Isabel", Apellido = "Romero", Email = "isabel@example.com", Telefono = "1234567899", FechaRegistro = DateTime.Now.AddDays(-1) },
            new Cliente { Nombre = "Wenaloco", Apellido = "Anashe", Email = "isabel@example.com", Telefono = "1234567899", FechaRegistro = DateTime.Now.AddDays(-1) }
                };

                _context.Clientes.AddRange(clientes);
                await _context.SaveChangesAsync();
            }

            // Ordenamiento
            ViewBag.NombreSortParm = string.IsNullOrEmpty(sortOrder) ? "nombre_desc" : "";
            ViewBag.FechaSortParm = sortOrder == "fecha" ? "fecha_desc" : "fecha";

            var clientesQuery = from c in _context.Clientes select c;

            switch (sortOrder)
            {
                case "nombre_desc":
                    clientesQuery = clientesQuery.OrderByDescending(c => c.Nombre);
                    break;
                case "fecha":
                    clientesQuery = clientesQuery.OrderBy(c => c.FechaRegistro);
                    break;
                case "fecha_desc":
                    clientesQuery = clientesQuery.OrderByDescending(c => c.FechaRegistro);
                    break;
                default:
                    clientesQuery = clientesQuery.OrderBy(c => c.Nombre);
                    break;
            }

            var totalClientes = await clientesQuery.CountAsync();
            var totalPaginas = (int)Math.Ceiling((double)totalClientes / pageSize);

            var clientesPaginados = await clientesQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.PaginaActual = page;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.SortOrder = sortOrder;

            return View(clientesPaginados);
        }


        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Email,Telefono,FechaRegistro")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Email,Telefono,FechaRegistro")] Cliente cliente)
        {
            if (id != cliente.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Clientes.Any(e => e.Id == cliente.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
            if (cliente == null) return NotFound();

            bool tieneCitas = await _context.Citas.AnyAsync(cita => cita.ClienteId == id);
            if (tieneCitas)
            {
                ViewBag.TieneCitas = true;
                ViewBag.MensajeAdvertencia = "Este cliente tiene citas asociadas y no puede ser eliminado.";
            }

            return View(cliente);
        }

        // POST: Clientes/DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            bool tieneCitas = await _context.Citas.AnyAsync(cita => cita.ClienteId == id);
            if (tieneCitas)
            {
                TempData["Error"] = "No se puede eliminar el cliente porque tiene citas asociadas.";
                return View("Delete", cliente);
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cliente eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
