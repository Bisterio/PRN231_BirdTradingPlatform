using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class CustomerDashboardDTO
    {
        public List<ProductViewDTO?> TopSellingProducts { get; set; } = new List<ProductViewDTO>();
        public List<ProductViewDTO?> RecentProducts { get; set; } = new List<ProductViewDTO>();
    }
}
