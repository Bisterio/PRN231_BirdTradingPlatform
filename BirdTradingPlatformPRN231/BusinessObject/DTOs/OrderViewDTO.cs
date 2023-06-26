using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class OrderViewDTO
    {
        public long OrderId { get; set; }
        public string Address { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public string? Email { get; set; }
        public string Name { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public string? Phone { get; set; }
        public byte Status { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalAmountPreShipping { get; set; }
        public int TotalItem { get; set; }
        public decimal TotalShippingCost { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? StoreName { get; set; }
        public string? StoreAddress { get; set; }
        public List<OrderItemViewDTO?> OrderItems { get; set; } = new List<OrderItemViewDTO?>();
    }
}
