using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class OrderItemViewDTO
    {
        public long ProductId { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string Name { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public string? CategoryName { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}
