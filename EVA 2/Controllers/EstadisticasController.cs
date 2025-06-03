using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EVA_2.Data;
using Microsoft.EntityFrameworkCore;

namespace EVA_2.Controllers
{
    public class EstadisticasController : Controller
    {
        private readonly AppDBContext _context;

        public EstadisticasController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Estadisticas
        public async Task<IActionResult> Index(DateTime? fechaInicio, DateTime? fechaFin)
        {
            // Si no se especifica rango, usar último mes por defecto
            if (!fechaInicio.HasValue)
                fechaInicio = DateTime.Today.AddMonths(-1);

            if (!fechaFin.HasValue)
                fechaFin = DateTime.Today;

            // Consultar citas dentro del rango de fechas (inclusive)
            var citasFiltradas = _context.Citas
                .Include(c => c.Servicio)
                .Where(c => c.Fecha >= fechaInicio && c.Fecha <= fechaFin);

            // Aquí agrupamos y calculamos cantidad e ingresos juntos
            var estadisticasServicios = await citasFiltradas
               .GroupBy(c => new { c.ServicioId, c.Servicio.Nombre, c.Servicio.Precio })
               .Select(g => new
               {
                   ServicioId = g.Key.ServicioId,
                   Nombre = g.Key.Nombre,
                   Cantidad = g.Count(),
                   Ingresos = g.Count() * g.Key.Precio
               })
               .OrderByDescending(x => x.Cantidad) // ordenas por cantidad o por ingresos según prefieras
               .ToListAsync();

            ViewBag.FechaInicio = fechaInicio.Value.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin.Value.ToString("yyyy-MM-dd");
            ViewBag.EstadisticasServicios = estadisticasServicios;

            return View();
        }
    }
}
