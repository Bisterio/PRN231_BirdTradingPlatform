using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class TransactionRecord
    {
        public long TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Mode { get; set; } = null!;
        public byte Status { get; set; }
        public string Type { get; set; } = null!;
        public DateTime? UpdatedAt { get; set; }
        public long? OrderId { get; set; }
        public long? ProductId { get; set; }
        public long? StoreId { get; set; }
        public long? UserId { get; set; }

        public virtual Order? Order { get; set; }
        public virtual Product? Product { get; set; }
        public virtual Store? Store { get; set; }
        public virtual UserAccount? User { get; set; }
    }
}
