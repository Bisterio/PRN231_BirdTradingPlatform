using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }

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
        public long? StoreId { get; set; }
        public long? UserId { get; set; }

        public virtual Store? Store { get; set; }
        public virtual UserAccount? User { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
