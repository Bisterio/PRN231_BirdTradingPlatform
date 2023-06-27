using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class Invoice
    {
        public Invoice()
        {
            Orders = new HashSet<Order>();
        }

        public long InvoiceId { get; set; }
        public string Address { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public string? Email { get; set; }
        public byte IsPaid { get; set; }
        public string Name { get; set; } = null!;
        public string? Note { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string? Phone { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalAmountPreShipping { get; set; }
        public int TotalItem { get; set; }
        public decimal TotalShippingCost { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long? UserId { get; set; }

        public virtual UserAccount? User { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
