using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ProductViewDTO
    {
        public long ProductId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? Name { get; set; }
        public byte Status { get; set; }
        public int Stock { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CategoryName { get; set; }
        public long? StoreId { get; set; }
        public string? StoreName { get; set; }
        public string? StoreAddress { get; set; }
        public string? StoreDescription { get; set; }
        public string? StoreLogoImage { get; set; }
        public string? StoreCoverImage { get; set; }

        public List<ProductViewDTO> RelatedProducts { get; set; } = new List<ProductViewDTO>();
    }
}
