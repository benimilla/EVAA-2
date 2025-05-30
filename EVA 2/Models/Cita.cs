namespace EVA_2.Models
{
    public class Cita
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public string Descripcion { get; set; }
        public int ClienteId { get; set; }
        public int ServicioId { get; set; }
        public string Estado { get; set; } // Estado de la cita (Pendiente, Confirmada, Cancelada)
        public string Comentarios { get; set; } // Comentarios adicionales sobre la cita
    }
}
