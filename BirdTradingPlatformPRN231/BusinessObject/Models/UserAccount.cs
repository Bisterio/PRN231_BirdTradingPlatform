using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class UserAccount
    {
        public UserAccount()
        {
            Invoices = new HashSet<Invoice>();
            Stores = new HashSet<Store>();
        }

        public long UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Email { get; set; } = null!;
        public byte EmailVerified { get; set; }
        public string Name { get; set; } = null!;
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public string Role { get; set; } = null!;
        public byte Status { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long? StoreId { get; set; }

        public virtual Store? Store { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<Store> Stores { get; set; }
    }
}
