using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class CartItem
    {
        public long ProductId { get; set; }
        public long UserId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual UserAccount User { get; set; } = null!;
    }
}
