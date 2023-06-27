using BusinessObject.Models;
using BusinessObject.DTOs;
using DataAccess;
using Repository.Interface;

namespace Repository.Implementation
{
    public class InvoiceRepository : IInvoiceRepository
    {
        public List<InvoiceViewDTO?> GetCurrentUserInvoices(long currentUserId)
        {
            List<InvoiceViewDTO?> invoicesByUser = InvoiceDAO
                .GetInvoicesByCurrentUser(currentUserId)
                .Select(x => Mapper.ToInvoiceViewDTO(x))
                .ToList();

            return invoicesByUser;
        }

        public InvoiceViewDTO? GetInvoiceDetailCustomer(long invoiceId, long currentUserId)
        {
            Invoice? invoiceEntity = InvoiceDAO.GetInvoiceByIdAndUserId(invoiceId, currentUserId);
            if (invoiceEntity == null) return null;

            InvoiceViewDTO? invoiceDTO = Mapper.ToInvoiceViewDTO(invoiceEntity);

            foreach (Order orderEntity in invoiceEntity.Orders)
            {
                OrderViewDTO? orderDTO = Mapper.ToOrderViewDTO(orderEntity);
                List<OrderItemViewDTO?> orderItemDTO = orderEntity.OrderItems
                    .Select(x => Mapper.ToOrderItemViewDTO(x))
                    .ToList();
                orderDTO.OrderItems = orderItemDTO;
                invoiceDTO.Orders.Add(orderDTO);
            }

            return invoiceDTO;
        }
    }
}
