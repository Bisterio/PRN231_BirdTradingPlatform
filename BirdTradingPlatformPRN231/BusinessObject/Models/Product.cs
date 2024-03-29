﻿using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public long ProductId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string Name { get; set; } = null!;
        public byte Status { get; set; }
        public int Stock { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long? CategoryId { get; set; }
        public long? StoreId { get; set; }

        public virtual Category? Category { get; set; }
        public virtual Store? Store { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
