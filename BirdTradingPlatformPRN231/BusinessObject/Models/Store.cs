using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class Store
    {
        public Store()
        {
            Products = new HashSet<Product>();
            TransactionRecords = new HashSet<TransactionRecord>();
            UserAccounts = new HashSet<UserAccount>();
        }

        public long StoreId { get; set; }
        public string Address { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public string Name { get; set; } = null!;
        public byte Status { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<TransactionRecord> TransactionRecords { get; set; }
        public virtual ICollection<UserAccount> UserAccounts { get; set; }
    }
}
