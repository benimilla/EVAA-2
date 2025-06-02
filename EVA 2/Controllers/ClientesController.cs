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

        // Index con paginación y orden (por nombre o fecha)
        public async Task<IActionResult> Index(string sortOrder, int page = 1)
        {
            ViewBag.NombreSortParm = string.IsNullOrEmpty(sortOrder) ? "nombre_desc" : "";
            ViewBag.FechaSortParm = sortOrder == "fecha" ? "fecha_desc" : "fecha";

            var clientes = from c in _context.Clientes select c;

            switch (sortOrder)
            {
                case "nombre_desc":
                    clientes = clientes.OrderByDescending(c => c.Nombre);
                    break;
                case "fecha":
                    clientes = clientes.OrderBy(c => c.FechaRegistro);
                    break;
                case "fecha_desc":
                    clientes = clientes.OrderByDescending(c => c.FechaRegistro);
                    break;
                default:
                    clientes = clientes.OrderBy(c => c.Nombre);
                    break;
            }

            var totalClientes = await clientes.CountAsync();
            var totalPaginas = (int)Math.Ceiling((double)totalClientes / pageSize);

            var clientesPaginados = await clientes
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.PaginaActual = page;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.SortOrder = sortOrder;

            return View(clientesPaginados);
        }

        // Detalles
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // Crear GET
        public IActionResult Create()
        {
            return View();
        }

        // Crear POST
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

        // Editar GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // Editar POST
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
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // Delete GET - Confirmar eliminación
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
            if (cliente == null) return NotFound();

            // Verificar si tiene citas asociadas para mostrar advertencia
            bool tieneCitas = await _context.Citas.AnyAsync(cita => cita.ClienteId == id);
            if (tieneCitas)
            {
                ViewBag.TieneCitas = true;
                ViewBag.MensajeAdvertencia = "Este cliente tiene citas asociadas y no puede ser eliminado.";
            }

            return View(cliente);
        }

        // Delete POST
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