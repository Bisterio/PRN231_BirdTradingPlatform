using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class StoreDashboardDTO
    {
        public decimal TotalSells { get; set; }
        public int CountCompletedOrders { get; set; }
        public int CountPendingOrders { get; set;}
        public List<OrderViewDTO?> RecentOrders { get; set; } = new List<OrderViewDTO>();
        public List<ProductViewDTO?> TopSellingProducts { get; set; } = new List<ProductViewDTO>();
    }
}
