using System.ComponentModel.DataAnnotations;

namespace EVA_2.Models
{
    public class Estado
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        // Relación inversa: una lista de Citas que usan este Estado (opcional)
        public ICollection<Cita>? Citas { get; set; }
    }
}
