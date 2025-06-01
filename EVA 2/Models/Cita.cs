namespace EVA_2.Models
{
    public class Cita
    {
        public int Id { get; set; }

        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; } = null!;

        public int ServicioId { get; set; }
        public Servicio Servicio { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }

        public string? Comentarios { get; set; }
    }
}