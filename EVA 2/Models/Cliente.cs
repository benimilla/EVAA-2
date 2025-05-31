namespace EVA_2.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public DateTime FechaRegistro { get; set; }
        //aloo
    }
}
