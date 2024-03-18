using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ClientOrderViewListDTO
    {
        public List<OrderViewDTO?> OrdersPaginated { get; set; } = new List<OrderViewDTO?>();
        public int Page { get; set; }
        public int Size { get; set; }
        public int TotalCount { get; set; }
        public int TotalPage { get; set; }
        public int Status { get; set; }
        public List<int>? PageNumbers { get; set; }
        public string? OrderIdSearch { get; set; }
        public byte IsReported { get; set; }
    }
}
