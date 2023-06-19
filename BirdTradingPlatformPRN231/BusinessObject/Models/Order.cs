using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
            TransactionRecords = new HashSet<TransactionRecord>();
        }

        public long OrderId { get; set; }
        public string Address { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public string? Phone { get; set; }
        public byte Status { get; set; }
        public int TotalItem { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long? UserId { get; set; }

        public virtual UserAccount? User { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<TransactionRecord> TransactionRecords { get; set; }
    }
}
