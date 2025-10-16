using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api_producto.Models
{
    public class ArticuloAddImagesDto
    {
        [Required]
        public List<string> Imagenes { get; set; }
    }
}
