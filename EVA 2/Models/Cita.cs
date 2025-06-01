namespace EVA_2.Models
{
    public class Cita
    {
        public int Id { get; set; }

        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; } = null!;

        public int ServicioId { get; set; }
        public Servicio? Servicio { get; set; } = null!;

    public int EstadoId { get; set; }            // Clave foránea
    public Estado Estado { get; set; } = null!;  // Propiedad de navegación

        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public string Estado { get; set; } = null!; // "Pendiente", "Confirmada", "Completada", "Cancelada", "Mas texto"
        public string? Comentarios { get; set; }
    }
}
