using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ClientInvoiceViewListDTO
    {
        public List<InvoiceViewDTO?> InvoicesPaginated { get; set; } = new List<InvoiceViewDTO?>();
        public int Page { get; set; }
        public int Size { get; set; }
        public int TotalCount { get; set; }
        public int TotalPage { get; set; }
        public List<int>? PageNumbers { get; set; }
    }
}
