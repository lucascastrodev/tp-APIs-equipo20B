using System.ComponentModel.DataAnnotations;

namespace api_producto.Models
{
    public class ArticuloCreateDto
    {
        [Required, StringLength(50)]
        public string Codigo { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "IdMarca debe ser mayor a 0.")]
        public int IdMarca { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "IdCategoria debe ser mayor a 0.")]
        public int IdCategoria { get; set; }

        [Required, Range(0, double.MaxValue, ErrorMessage = "El precio no puede ser negativo.")]
        public decimal Precio { get; set; }
    }
}
