using System.ComponentModel.DataAnnotations;

namespace EVA_2.Models
{
    public class Cita
    {
        public int Id { get; set; }

        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; } = null!;

        [Display(Name = "Servicio")]
        public int ServicioId { get; set; }
        public Servicio? Servicio { get; set; } = null!;

        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public string Estado { get; set; } = null!; // "Pendiente", "Confirmada", "Completada", "Cancelada", "Mas texto"
        public string? Comentarios { get; set; }
        public DateTime? FechaCambioEstado { get; set; }
    }
}