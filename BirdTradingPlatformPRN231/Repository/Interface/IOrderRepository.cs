using BusinessObject.Common;
using BusinessObject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IOrderRepository
    {
        public APIResult<string> CreateNewOrders(OrderCreateDTO newOrders, long currentUserId);
        public ClientOrderViewListDTO GetCurrentUserOrders(int page, byte status, long currentUserId);
        public ClientOrderViewListDTO GetCurrentStoreOrders(int page, byte status, long currentUserId, string orderIdSearch);
        public OrderViewDTO? GetOrderDetailCustomer(long orderId, long currentUserId);
        public OrderViewDTO? GetOrderDetailStore(long orderId, long currentUserId);
        public APIResult<string> CancelOrderDetailCustomer(long orderId, long currentUserId);
        public APIResult<string> DeliverOrder(long orderId, long currentStoreStaffId);
        public APIResult<string> ConfirmOrderDelivered(long orderId, long currentStoreStaffId);
        public APIResult<string> RefundRequest(long orderId, long currentUserId, string refundReason);
        public APIResult<string> RefundDecline(long orderId, long currentStoreStaffId);
        public APIResult<string> RefundAccept(long orderId, long currentStoreStaffId);
    }
}
