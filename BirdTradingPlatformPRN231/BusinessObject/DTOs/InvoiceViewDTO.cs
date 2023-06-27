using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class InvoiceViewDTO
    {
        public long InvoiceId { get; set; }
        public string? Address { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Email { get; set; }
        public byte IsPaid { get; set; }
        public string? Name { get; set; }
        public string? Note { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Phone { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalAmountPreShipping { get; set; }
        public int TotalItem { get; set; }
        public decimal TotalShippingCost { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<OrderViewDTO?> Orders { get; set; } = new List<OrderViewDTO?>();
    }
}
