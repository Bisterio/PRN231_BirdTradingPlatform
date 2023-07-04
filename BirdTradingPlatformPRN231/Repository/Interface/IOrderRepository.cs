using BusinessObject.Common;
using BusinessObject.DTOs;
using BusinessObject.Models;
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
        public ClientOrderViewListDTO GetCurrentAdminOrders(int page, byte isReported);
        public OrderViewDTO? GetOrderDetailCustomer(long orderId, long currentUserId);
        public OrderViewDTO? GetOrderDetailStore(long orderId, long currentUserId);
        public OrderViewDTO? GetOrderDetailAdmin(long orderId);
        public APIResult<string> CancelOrderDetailCustomer(long orderId, long currentUserId, string cancelReason);
        public APIResult<string> RequestCancelOrderDetailCustomer(long orderId, long currentUserId, string cancelReason);
        public APIResult<string> ApproveOrderStore(long orderId, long currentUserId);
        public APIResult<string> CancelOrderDetailStore(long orderId, long currentUserId, string cancelReason);
        public APIResult<string> ApproveOrderCancelRequestStore(long orderId, long currentUserId);
        public APIResult<string> DeclineOrderCancelRequestStore(long orderId, long currentUserId);
        public APIResult<string> DeliverOrder(long orderId, long currentStoreStaffId);
        public APIResult<string> ConfirmOrderDelivered(long orderId, long currentStoreStaffId);
        public APIResult<string> RefundRequest(long orderId, long currentUserId, string refundReason);
        public APIResult<string> RefundDecline(long orderId, long currentStoreStaffId);
        public APIResult<string> RefundAccept(long orderId, long currentStoreStaffId);
        public APIResult<string> Report(long orderId, long currentUserId, string reportReason);
        public APIResult<string> ResolveReport(long orderId);
        public APIResult<string> ApproveRefundReport(long orderId);
        public Order GetOrderForEmail(long id);
    }
}
