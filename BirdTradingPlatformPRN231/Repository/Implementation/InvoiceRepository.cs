using BusinessObject.Models;
using BusinessObject.DTOs;
using DataAccess;
using Repository.Interface;

namespace Repository.Implementation
{
    public class InvoiceRepository : IInvoiceRepository
    {
        public ClientInvoiceViewListDTO GetCurrentUserInvoices(int page, long currentUserId)
        {
            // Handle query data
            int size = 12;
            page = page == 0 ? 1 : page;

            // Get paginated invoices list
            List<InvoiceViewDTO?> invoicesByUser = InvoiceDAO
                .GetInvoicesByCurrentUser(page, size, currentUserId)
                .Select(x => Mapper.ToInvoiceViewDTO(x))
                .ToList();

            // Get count of invoices
            int invoiceCount = InvoiceDAO.CountInvoicesByCurrentUser(currentUserId);
            int totalPages = (int)Math.Ceiling((double)invoiceCount / size);
            List<int> pageNumbers = new List<int>();
            if (totalPages > 0)
            {
                int start = Math.Max(1, page - 2);
                int end = Math.Min(page + 2, totalPages);

                if (totalPages > 5)
                {
                    if (end == totalPages) start = end - 4;
                    else if (start == 1) end = start + 4;
                }
                else
                {
                    start = 1;
                    end = totalPages;
                }
                pageNumbers = Enumerable.Range(start, end - start + 1).ToList();
            }

            return new ClientInvoiceViewListDTO()
            {
                TotalPage = totalPages,
                TotalCount = invoiceCount,
                PageNumbers = pageNumbers,
                InvoicesPaginated = invoicesByUser,
                Page = page,
                Size = size,
            };
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
