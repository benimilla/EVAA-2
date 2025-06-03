using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EVA_2.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EVA_2.Controllers
{
    public class EstadisticasController : Controller
    {
        private readonly AppDBContext _context;

        public EstadisticasController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime? fechaInicio, DateTime? fechaFin, string filtro)
        {
            if (!fechaInicio.HasValue)
                fechaInicio = DateTime.Today.AddMonths(-1);
            if (!fechaFin.HasValue)
                fechaFin = DateTime.Today;

            var citasFiltradas = _context.Citas
                .Include(c => c.Servicio)
                .Where(c => c.Fecha >= fechaInicio && c.Fecha <= fechaFin);

            List<dynamic> estadisticasServicios;

            if (filtro == "masSolicitados")
            {
                estadisticasServicios = await citasFiltradas
                    .GroupBy(c => new { c.ServicioId, c.Servicio.Nombre, c.Servicio.Precio })
                    .Select(g => new
                    {
                        ServicioId = g.Key.ServicioId,
                        Nombre = g.Key.Nombre,
                        Cantidad = g.Count(),
                        Ingresos = g.Count() * g.Key.Precio
                    })
                    .OrderByDescending(x => x.Cantidad)
                    .ToListAsync<dynamic>();
            }
            else
            {
                estadisticasServicios = await _context.Servicios
                    .Select(s => new
                    {
                        ServicioId = s.Id,
                        Nombre = s.Nombre,
                        Cantidad = citasFiltradas.Count(c => c.ServicioId == s.Id),
                        Ingresos = citasFiltradas.Where(c => c.ServicioId == s.Id).Count() * s.Precio
                    })
                    .OrderByDescending(x => x.Cantidad)
                    .ToListAsync<dynamic>();
            }

            ViewBag.FechaInicio = fechaInicio.Value.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin.Value.ToString("yyyy-MM-dd");
            ViewBag.Filtro = filtro;
            ViewBag.EstadisticasServicios = estadisticasServicios;

            return View();
        }
    }
}
