using BusinessObject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IInvoiceRepository
    {
        public ClientInvoiceViewListDTO GetCurrentUserInvoices(int page, long currentUserId);
        public InvoiceViewDTO? GetInvoiceDetailCustomer(long invoiceId, long currentUserId);
    }
}
