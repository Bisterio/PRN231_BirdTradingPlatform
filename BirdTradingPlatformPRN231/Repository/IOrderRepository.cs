using BusinessObject.Common;
using BusinessObject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IOrderRepository
    {
        APIResult<String> CreateNewOrders(OrderCreateDTO newOrders, long currentUserId);
        public List<OrderViewDTO> GetCurrentUserOrders(byte status, long currentUserId);
        public OrderViewDTO GetOrderDetailCustomer(long orderId, long currentUserId);
    }
}
