using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class OrderItemCartDTO
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public long StoreId { get; set; }
    }
}
