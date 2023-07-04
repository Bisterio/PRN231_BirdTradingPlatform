using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ProductCreateDTO
    {
        [Required]
        public string? Description { get; set; } = null!;
        public string? Image { get; set; }
        [Required]
        [MaxLength(500)]
        public string Name { get; set; } = null!;
        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public decimal UnitPrice { get; set; }
        [Required]
        public long? CategoryId { get; set; }
        public long ProductId { get; set; }

        public List<Category> Categories { get; set; } = new List<Category>();
    }
}
