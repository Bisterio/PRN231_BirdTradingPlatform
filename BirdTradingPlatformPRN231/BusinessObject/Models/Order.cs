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
        public DateTime? CreatedAt { get; set; }
        public byte Status { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalAmountPreShipping { get; set; }
        public int TotalItem { get; set; }
        public decimal TotalShippingCost { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long? InvoiceId { get; set; }
        public long? StoreId { get; set; }
        public string? CancelReason { get; set; }
        public byte? IsReported { get; set; }
        public DateTime? RefundDuration { get; set; }
        public string? RefundReason { get; set; }
        public string? ReportedReason { get; set; }

        public virtual Invoice? Invoice { get; set; }
        public virtual Store? Store { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
