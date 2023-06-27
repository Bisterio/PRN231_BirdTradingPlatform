using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class Store
    {
        public Store()
        {
            Orders = new HashSet<Order>();
            Products = new HashSet<Product>();
            UserAccounts = new HashSet<UserAccount>();
        }

        public long StoreId { get; set; }
        public string Address { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public string Name { get; set; } = null!;
        public byte Status { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CoverImage { get; set; }
        public string? Description { get; set; }
        public string? LogoImage { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<UserAccount> UserAccounts { get; set; }
    }
}
