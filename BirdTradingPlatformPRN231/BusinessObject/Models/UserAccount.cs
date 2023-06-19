using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class UserAccount
    {
        public UserAccount()
        {
            CartItems = new HashSet<CartItem>();
            Orders = new HashSet<Order>();
            TransactionRecords = new HashSet<TransactionRecord>();
        }

        public long UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public string Role { get; set; } = null!;
        public byte Status { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long? StoreId { get; set; }

        public virtual Store? Store { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<TransactionRecord> TransactionRecords { get; set; }
    }
}
