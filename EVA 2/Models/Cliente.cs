using System;
using System.ComponentModel.DataAnnotations;

namespace EVA_2.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Telefono { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaRegistro { get; set; }
    }
}
