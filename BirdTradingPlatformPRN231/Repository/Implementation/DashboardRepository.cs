using BusinessObject.DTOs;
using DataAccess;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implementation
{
    public class DashboardRepository : IDashboardRepository
    {
        public StoreDashboardDTO GetStoreDashboard(long currentUser)
        {
            decimal totalSells = OrderDAO.GetTotalSells(currentUser);
            int countDeliveredOrders = OrderDAO.CountOrdersByCurrentStore(2, currentUser, "");
            int countPendingOrders = OrderDAO.CountOrdersByCurrentStore(1, currentUser, "");
            List<OrderViewDTO?> recentOrders = OrderDAO.GetOrdersByCurrentStore(1, 8, 0, currentUser, "")
                .Select(x => Mapper.ToOrderViewDTO(x))
                .ToList();
            List<ProductViewDTO?> topProducts = ProductDAO.GetTopProductsByStore(currentUser, 5)
                .Select(x => Mapper.ToProductViewDTO(x))
                .ToList();

            return new StoreDashboardDTO()
            {
                TotalSells = totalSells,
                CountCompletedOrders= countDeliveredOrders,
                CountPendingOrders= countPendingOrders,
                RecentOrders= recentOrders,
                TopSellingProducts = topProducts
            };
        }
    }
}
