using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class RefreshToken
    {
        public long TokenId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public string? Token { get; set; }
        public long? UserId { get; set; }

        public virtual UserAccount? User { get; set; }
    }
}
