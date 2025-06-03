using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EVA_2.Data;
using EVA_2.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EVA_2.Controllers
{
    public class ReporteCitasController : Controller
    {
        private readonly AppDBContext _context;

        public ReporteCitasController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime? desde, DateTime? hasta)
        {
            DateTime fechaInicio = desde ?? DateTime.Today.AddDays(-7);
            DateTime fechaFin = hasta?.AddDays(1).AddTicks(-1) ?? DateTime.Today.AddDays(1).AddTicks(-1);

            // Filtrar citas entre fechas
            var citas = await _context.VistaReporteCitas
                .Where(c => c.Fecha >= fechaInicio && c.Fecha <= fechaFin)
                .ToListAsync();

            // Totales por estado
            ViewBag.TotalPendientes = citas.Count(c => c.Estado == "Pendiente");
            ViewBag.TotalConfirmadas = citas.Count(c => c.Estado == "Confirmada");
            ViewBag.TotalCompletadas = citas.Count(c => c.Estado == "Completada");
            ViewBag.TotalCanceladas = citas.Count(c => c.Estado == "Cancelada");

            ViewBag.FechaInicio = fechaInicio.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin.ToString("yyyy-MM-dd");

            // Retornar la vista con el modelo filtrado
            return View(citas);
        }
    }
}
