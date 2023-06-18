using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Product
    {
        public Product()
        {
            CartItems = new HashSet<CartItem>();
            OrderItems = new HashSet<OrderItem>();
            TransactionRecords = new HashSet<TransactionRecord>();
        }

        public long ProductId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string Name { get; set; } = null!;
        public byte Status { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long? CategoryId { get; set; }
        public long? StoreId { get; set; }
        public int Stock { get; set; }

        public virtual Category? Category { get; set; }
        public virtual Store? Store { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<TransactionRecord> TransactionRecords { get; set; }
    }
}
