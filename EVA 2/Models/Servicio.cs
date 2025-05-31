using System.ComponentModel.DataAnnotations;
using EVA_2.Views.Shared;
using Microsoft.AspNetCore.Mvc;

namespace EVA_2.Models
{
    public class Servicio
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        [Range(1, 300, ErrorMessage = "La duración debe ser un número positivo")]
        public int Duracion { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string Descripcion { get; set; }

        [ModelBinder(BinderType = typeof(DecimalModelBinder))]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }

        public bool Activo { get; set; }
    }
}